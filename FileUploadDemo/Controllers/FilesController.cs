using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FileUploadDemo.Controllers
{
  public class FilesController : ApiController
  {

    public HttpResponseMessage Post([FromUri] string fileName)
    {
      if (!Request.Content.IsMimeMultipartContent("form-data"))
      {
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
      }

      var streamProvider = new MultipartFormDataStreamProvider();

      var task = Request.Content.ReadAsMultipartAsync(streamProvider);
      task.Wait();
      var bodyparts = task.Result;

      // The submitter field is the entity with a Content-Disposition header field with a "name" parameter with value "submitter"
      string submitter;
      if (!bodyparts.TryGetFormFieldValue("submitter", out submitter))
      {
        submitter = "unknown";
      }


      // Get a dictionary of local file names from stream provider.
      // The filename parameters provided in Content-Disposition header fields are the keys.
      // The local file names where the files are stored are the values.
      var bodyPartFileNames = streamProvider.BodyPartFileNames;

      foreach (KeyValuePair<string, string> file in bodyPartFileNames)
      {
        var fileInfo = new FileInfo(file.Value);
        var fileresult = new
            {
              FileName = file.Key,
              LocalPath = fileInfo.FullName,
              LastModifiedDate = fileInfo.LastWriteTimeUtc,
              Length = fileInfo.Length,
              Submitter = submitter
            };

        try
        {
          var savePath = GetSavePath(file.Key);
          if (File.Exists(savePath))
            File.Delete(savePath);
          File.Move(fileInfo.FullName, savePath);
        }
        catch (IOException ex)
        {
          Console.WriteLine(ex.Message);
          throw new HttpResponseException("A generic error occured");
        }

      }

      var response = new HttpResponseMessage
      {
        StatusCode = HttpStatusCode.Created,
        ReasonPhrase = "Submitter: " + submitter,
        Content = new StringContent("Submitter: " + submitter)
      };
      return response;
      }
      
    private static string GetSavePath(string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
      {
        fileName = Path.GetRandomFileName();
      }
      fileName = fileName.Replace("\"", "");

      string newPath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "uploads");
      if (!Directory.Exists(newPath))
      {
        Directory.CreateDirectory(newPath);
      }
      return Path.Combine(newPath, fileName);
    }

  }
}
