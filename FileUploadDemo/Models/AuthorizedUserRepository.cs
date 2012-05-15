using System.Collections.Generic;
using System.Linq;

namespace FileUploadDemo.Models
{
 public class AuthorizedUserRepository
 {
   public static IQueryable<User> GetUsers() {
   
      IList<User> users = new List<User>();   
      users.Add(new User("User1"));
      users.Add(new User("User2"));
      users.Add(new User("User3"));
      users.Add(new User("Administrator"));
   
      return users.AsQueryable();
   }
 }
}