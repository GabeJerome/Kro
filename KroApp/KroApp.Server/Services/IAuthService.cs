using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace KroApp.Server.Services
{
  public interface IAuthService
  {
    Task<SignInResult> LogIn(UserLogin model);
    Task<IdentityResult> RegisterUser(UserRegister model);
    Task<bool> UserExists(string usernameOrEmail);
    Task<User?> GetUser(string usernameOrEmail);
    string GenerateJwtToken(string email, string username, bool rememberMe);
  }
}
