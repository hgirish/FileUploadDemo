using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FileUploadDemo.Models;

namespace WebAPI
{
 public class IPHostValidationHandler : DelegatingHandler 
 {
   protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
   CancellationToken cancellationToken) {

    var context = request.Properties["MS_HttpContext"] as System.Web.HttpContextBase;
    string userIP = context.Request.UserHostAddress;

    var foundIP = AuthorizedIPRepository.GetAuthorizedIPs().FirstOrDefault(x => x == userIP);
    if (foundIP == null)
     return Task<HttpResponseMessage>.Factory.StartNew(() =>
     {
      return new HttpResponseMessage(HttpStatusCode.Forbidden)
      {
       Content = new StringContent("Unauthorized IP Address")
      };
     });

    return base.SendAsync(request, cancellationToken);

   }
 }
}