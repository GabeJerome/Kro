using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KroApp.Server.Services
{
  public class AuthService : IAuthService
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager)
    {
      _configuration = configuration;
      _userManager = userManager;
      _signInManager = signInManager;
    }

    public string GenerateJwtToken(string email, bool rememberMe)
    {
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var jwtKey = _configuration["Jwt:Key"];
      if (string.IsNullOrEmpty(jwtKey))
      {
        throw new InvalidOperationException("JWT Key is not configured. Please check your user secrets.");
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: "Kro.com",
          audience: "Kro.com",
          claims: claims,
          expires: rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.Now.AddHours(4),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<SignInResult> LogIn(UserLogin model)
    {
      return await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
    }

    public async Task<IdentityResult> RegisterUser(UserRegister model)
    {
      var user = new User { UserName = model.Username, Email = model.Email };
      return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<bool> UserExists(string email)
    {
      return await _userManager.FindByEmailAsync(email) != null;
    }
  }
}
