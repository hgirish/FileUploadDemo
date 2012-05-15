using System.Collections.Generic;
using System.Linq;

namespace FileUploadDemo.Models
{
 public class AuthorizedIPRepository
 {
    public static IQueryable<string> GetAuthorizedIPs() {
    
      var ips = new List<string>();

      ips.Add("127.0.0.1");
      ips.Add("::1");
      
      return ips.AsQueryable();
    }
 }
}