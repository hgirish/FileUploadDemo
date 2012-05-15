using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FileUploadDemo.Models;

namespace FileUploadDemo.Secure
{
 public class TokenValidationHandler : DelegatingHandler 
 {
  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
   CancellationToken cancellationToken) 
  {

    string token;

    try
    {
      token = request.Headers.GetValues("Authorization-Token").FirstOrDefault();
    }
    catch (Exception)
    {
      token = string.Empty;
    }


    if (String.IsNullOrEmpty(token))
      return Task<HttpResponseMessage>.Factory.StartNew(() =>
      new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        Content = new StringContent("Missing Authorization-Token")
      });

   try
   {
    var foundUser = AuthorizedUserRepository.GetUsers().FirstOrDefault(x => x.Name == RSAClass.Decrypt(token));
    if (foundUser == null)
     return Task<HttpResponseMessage>.Factory.StartNew(() => 
     new HttpResponseMessage(HttpStatusCode.Forbidden){
     Content = new StringContent("Unauthorized User")
     });
   }
   catch (RSAClass.RSAException)
   {
    return Task<HttpResponseMessage>.Factory.StartNew(() => 
    new HttpResponseMessage(HttpStatusCode.InternalServerError){
    Content = new StringContent("Error encountered while attempting to process authorization token")
    });
   }
   return base.SendAsync(request, cancellationToken);
  }
 }
}