using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace KroApp.Server.Services
{
  public interface IAuthService
  {
    Task<SignInResult> LogIn(UserLogin model);
    Task<IdentityResult> RegisterUser(UserRegister model);
    Task<bool> UserExists(string email);
    string GenerateJwtToken(string email, bool rememberMe);
  }
}
