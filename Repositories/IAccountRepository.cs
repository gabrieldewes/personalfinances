using System.Collections.Generic;
using System.Security.Claims;
using MyApp.Models;

namespace MyApp.Repositories {
    public interface IAccountRepository {
        ClaimsPrincipal Get(string username);
        List<User> GetAll();
        long Add(User u);
        User GetById(long id);
        User GetByUsername(string username);
        long Update(User u);
        long Delete(long id);
        bool UserExists(long id);
        bool FieldAlreadyInUse(string field, string value);
    }
}