using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FileUploadDemo.Secure;
using NUnit.Framework;

namespace FileUploadDemo.Tests
{
  // ReSharper disable InconsistentNaming 

  [TestFixture]
  public class TokenHandlerTests
  {
    string myUrl = "http://localhost:12244/api/Files";
    string token = "57,46,60,70,93,230,85,33,98,19,10,46,84,91,218,43,207,42,159,167,5,25,157,4,224,142,235,8,160,199,123,100,107,58,37,204,133,81,138,196,237,190,56,119,158,7,224,89,84,85,208,169,44,179,102,218,55,60,76,134,144,22,208,230,165,179,83,125,86,57,224,42,29,58,188,45,73,33,160,87,165,105,131,139,132,137,209,67,92,36,168,73,176,205,251,48,240,228,14,39,197,36,42,21,216,242,172,4,160,234,138,77,156,28,191,63,111,207,221,31,103,213,58,62,186,123,221,230";


    [Test]
    public void VerifyToken()
    {
      const string token = "dummy";
      var encryptedToken = RSAClass.Encrypt(token);
      var decryptedToken = RSAClass.Decrypt(encryptedToken);

      Assert.AreEqual(token, decryptedToken);
      Console.WriteLine("Token for dummy");
      Console.WriteLine(encryptedToken);
    }

    [Test]
    public void TestMissingAuthorizationToken()
    {
      var request = GetRequest("GET", "application/json", "http://localhost:12244/api/Files");
      request.Headers.Remove("Authorization-Token");
      var expectedMessage = "Missing Authorization-Token";
      var expectedStatus = HttpStatusCode.BadRequest;
      HttpWebResponse response;
      try
      {
        response = request.GetResponse() as HttpWebResponse;
      }
      catch (WebException e)
      {

        response = e.Response as HttpWebResponse;
      }

      Assert.AreEqual(expectedMessage, UnpackResponse(response));
      Assert.AreEqual( expectedStatus, response.StatusCode);
    }

    [Test]
    public void TestTokenAuthorization()
    {
      var request = GetRequest("GET", "application/json", "http://localhost:12244/api/Files");
      var expectedMessage = "Success!";
      var expectedStatus = HttpStatusCode.OK;
      HttpWebResponse response;
      try
      {
        response = request.GetResponse() as HttpWebResponse;
      }
      catch (WebException e)
      {

        response = e.Response as HttpWebResponse;
      }

      Assert.AreEqual(expectedMessage, UnpackResponse(response));
      Assert.AreEqual(expectedStatus, response.StatusCode);
    }

    [Test]
    public void TestUnauthorizedUser()
    {
      var request = GetRequest("GET", "application/json", "http://localhost:12244/api/Files");
      request.Headers.Remove("Authorization-Token");
      request.Headers.Add("Authorization-Token", RSAClass.Encrypt("baduser"));

      var expectedMessage = "Unauthorized User";
      var expectedStatus = HttpStatusCode.Forbidden;
      HttpWebResponse response;
      try
      {
        response = request.GetResponse() as HttpWebResponse;
      }
      catch (WebException e)
      {

        response = e.Response as HttpWebResponse;
      }

      Assert.AreEqual(expectedMessage, UnpackResponse(response));
      Assert.AreEqual(expectedStatus, response.StatusCode);
    }

   
    string UnpackResponse(WebResponse response)
    {
      var dataStream = response.GetResponseStream();
      if (dataStream != null)
      {
        var reader = new StreamReader(dataStream);
        return reader.ReadToEnd();
      }
      return null;
    }

    WebRequest GetRequest(string method, string contentType, string endPoint)
    {
      var request = (HttpWebRequest)WebRequest.Create(endPoint);
      request.Method = method;
      request.ContentType = contentType;
      request.Credentials = CredentialCache.DefaultCredentials;
      request.KeepAlive = true;

      ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
      request.Headers.Add("Authorization-Token", token);

      return request;
    }

    WebRequest GetRequest(string method, string contentType, string endPoint, string content)
    {
      var request = GetRequest(method, contentType, endPoint);
      var dataArray = Encoding.UTF8.GetBytes(content);
      request.ContentLength = dataArray.Length;
      var requestStream = request.GetRequestStream();
      requestStream.Write(dataArray, 0, dataArray.Length);
      requestStream.Flush();
      requestStream.Close();

      return request;
    }


    public WebRequest GetFileRequest(string url, 
    string file, string paramName, 
    string contentType, NameValueCollection nvc)
    {
      Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
      string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
      byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

    var wr = (HttpWebRequest)WebRequest.Create(url);
    wr.ContentType = "multipart/form-data; boundary=" + boundary;
    wr.Method = "POST";
    wr.KeepAlive = true;
    wr.Credentials = CredentialCache.DefaultCredentials;
    wr.Headers.Add("Authorization-Token", token);
      var rs = wr.GetRequestStream();

      const string formdataTemplate 
      = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
      foreach (string key in nvc.Keys)
      {
        rs.Write(boundarybytes, 0, boundarybytes.Length);
        string formitem = string.Format(formdataTemplate, key, nvc[key]);
        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
        rs.Write(formitembytes, 0, formitembytes.Length);
      }
      rs.Write(boundarybytes, 0, boundarybytes.Length);

      const string headerTemplate = 
      "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
      var header = string.Format(headerTemplate, paramName, file, contentType);
      var headerbytes = Encoding.UTF8.GetBytes(header);
      rs.Write(headerbytes, 0, headerbytes.Length);

      var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
      var buffer = new byte[4096];
      int bytesRead;
      while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
      {
        rs.Write(buffer, 0, bytesRead);
      }
      fileStream.Close();

      byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
      rs.Write(trailer, 0, trailer.Length);
      rs.Close();
      return wr;

     
    }

    [Test]
    public void TestUploadFiles()
    {
      var nvc = new NameValueCollection
                  {{"id", "TTR"}, 
                  {"subfolder", "tessting"}, 
                  {"submitter", "girish"}, 
                  {"btn-submit-photo", "Upload"}};

     var wr = GetFileRequest(myUrl,@"myFileName.txt", "file", "plain/text", nvc);

      WebResponse wresp = null;
      try
      {
        wresp = wr.GetResponse();
        Console.WriteLine(string.Format(
         "File uploaded, server response is: {0}", UnpackResponse(wresp)));
      }
      catch (Exception ex)
      {
        Console.WriteLine(string.Format("Error uploading file: {0}", ex.Message));
        if (wresp != null)
        {
          wresp.Close();
        }
      }
    }

  }

  // ReSharper restore InconsistentNaming 
}