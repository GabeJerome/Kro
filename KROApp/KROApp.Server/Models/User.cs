using Microsoft.AspNetCore.Identity;

namespace KROApp.Server.Models
{
  public class User : IdentityUser
  {
    public int Id;

    public string Email { get; set; }
  }
}
