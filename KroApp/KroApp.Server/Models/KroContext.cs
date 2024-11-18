using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KroApp.Server.Models
{
  public class KroContext : IdentityDbContext<User>
  {
    public KroContext(DbContextOptions<KroContext> options) : base(options) { }

  }
}
