using System.Collections.Generic;
using System.Net.Http;

namespace FileUploadDemo
{
  public static  class HttpContentExtensions
  {
    public static bool TryGetFormFieldValue(this IEnumerable<HttpContent> contents, string dispositionName, out string formFieldValue)
    {
      HttpContent content = contents.FirstDispositionNameOrDefault(dispositionName);
      if (content != null)
      {
        formFieldValue = content.ReadAsStringAsync().Result;
        return true;
      }

      formFieldValue = null;
      return false;
    }
  }
}