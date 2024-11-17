using KroApp.Server.Models.Users;
using KroApp.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KroApp.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAuthService _authService;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IAuthService authService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegister model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = new User { UserName = model.Email, Email = model.Email };
      var result = await _userManager.CreateAsync(user, model.Password);

      if (!result.Succeeded)
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
      }

      var token = _authService.GenerateJwtToken(model.Email, false);

      return Ok(new { Message = "User registered successfully", Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

      if (result.IsLockedOut)
      {
        return StatusCode(423, new { Message = "Account is locked. Please try again later." });
      }

      if (!result.Succeeded)
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Unauthorized(new SerializableError(ModelState));
      }

      var token = _authService.GenerateJwtToken(model.Email, model.RememberMe);

      return Ok(new { Message = "Login successful", Token = token });
    }

  }
}
