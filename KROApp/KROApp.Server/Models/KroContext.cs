using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KROApp.Server.Models
{
  public class KroContext(DbContextOptions<KroContext> options) : IdentityDbContext<User>(options)
  {
    public DbSet<User> Users { get; set; }
  }
}
