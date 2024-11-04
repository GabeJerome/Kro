﻿using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KroApp.Server.Models
{
    public class KroContext(DbContextOptions<KroContext> options) : IdentityDbContext<User>(options)
  {
    
  }
}
