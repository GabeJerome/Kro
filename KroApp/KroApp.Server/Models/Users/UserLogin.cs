using System.ComponentModel.DataAnnotations;

namespace KroApp.Server.Models.Users
{
    public class UserLogin
    {
    [Required]
    public required string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;
  }
}
