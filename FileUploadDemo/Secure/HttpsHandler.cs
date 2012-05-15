using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI
{
 public class HttpsHandler : DelegatingHandler 
 { 
  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
   CancellationToken cancellationToken) 
  {
   if (!String.Equals(request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
   {
    return Task<HttpResponseMessage>.Factory.StartNew(() =>
    {
     return new HttpResponseMessage(HttpStatusCode.BadRequest)
     {
      Content = new StringContent("HTTPS Required")
     };
    });
   }
   return base.SendAsync(request, cancellationToken);
  }
 }
}