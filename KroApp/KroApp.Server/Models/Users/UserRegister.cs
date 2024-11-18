using System.ComponentModel.DataAnnotations;

namespace KroApp.Server.Models.Users
{
  public class UserRegister
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
  }
}
