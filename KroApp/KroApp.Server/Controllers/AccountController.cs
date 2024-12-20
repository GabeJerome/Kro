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
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegister model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (await _authService.UserExists(model.Username))
      {
        ModelState.AddModelError(string.Empty, "An account with this username already exists.");
        return BadRequest(ModelState);
      }
      
      if (await _authService.UserExists(model.Email))
      {
        ModelState.AddModelError(string.Empty, "An account with this email already exists.");
        return BadRequest(ModelState);
      }

      var result = await _authService.RegisterUser(model);

      if (!result.Succeeded)
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
      }

      var token = _authService.GenerateJwtToken(model.Email, model.Username, false);

      return Ok(new { Message = "User registered successfully", Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _authService.LogIn(model);

      if (result.IsLockedOut)
      {
        return StatusCode(423, new { Message = "Account is locked. Please try again later." });
      }

      if (!result.Succeeded)
      {
        ModelState.AddModelError(string.Empty, "Username or password is incorrect. Please try again.");
        return Unauthorized(new SerializableError(ModelState));
      }

      var user = await _authService.GetUser(model.Username);
      if (user == null) {
        return StatusCode(500, new { Message = "Server encountered an internal error with your account." });
      }

      var token = _authService.GenerateJwtToken(user.Email, model.Username, model.RememberMe);

      return Ok(new { Message = "Login successful", Token = token });
    }

  }
}
