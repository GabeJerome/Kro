using KroApp.Server.Models.Ingredients;
using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace KroApp.Server.Models.SeedData
{
  public class SeedDatabase
  {
    public static void Initialize(IServiceProvider serviceProvider, Assembly assembly)
    {
      using var context = new KroContext(serviceProvider.GetRequiredService<DbContextOptions<KroContext>>());

      if (!context.Users.Any())
      {
        SeedUsers(context, assembly);
        context.SaveChanges();
      }

      if (!context.Ingredients.Any())
      {
        SeedIngredients(context, assembly);
        context.SaveChanges();
      }

      if (!context.UserIngredients.Any())
      {
        SeedUserIngredients(context, assembly);
        context.SaveChanges();
      }

      context.SaveChanges();
    }

    public static void SeedUsers(KroContext context, Assembly assembly)
    {
      string sourceName = "KroApp.Server.Models.SeedData.Users.csv";

      using var stream = assembly.GetManifestResourceStream(sourceName);
      if (stream == null)
      {
        Debug.WriteLine($"Resource not found: {sourceName}");
        return;
      }

      using var reader = new StreamReader(stream);
      reader.ReadLine();

      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        var values = line.Split(",");
        if (values.Length != 15)
        {
          Debug.WriteLine($"Invalid row format: {line}");
          continue;
        }
        
        context.Users.Add(new User
        {
          Id = values[0],
          UserName = values[1],
          Email = values[2],
          NormalizedUserName = values[3],
          NormalizedEmail = values[4],
          EmailConfirmed = Convert.ToBoolean(values[5]),
          PasswordHash = values[6],
          SecurityStamp = values[7],
          ConcurrencyStamp = values[8],
          PhoneNumber = values[9],
          PhoneNumberConfirmed = Convert.ToBoolean(values[10]),
          TwoFactorEnabled = Convert.ToBoolean(values[11]),
          LockoutEnd = Convert.ToDateTime(values[12]),
          LockoutEnabled = Convert.ToBoolean(values[13]),
          AccessFailedCount = Convert.ToInt32(values[14])
        });
      }

      Debug.WriteLine("Users seeded successfully.");
    }

    public static void SeedIngredients(KroContext context, Assembly assembly)
    {
      string sourceName = "KroApp.Server.Models.SeedData.Ingredients.csv";

      using var stream = assembly.GetManifestResourceStream(sourceName);
      if (stream == null)
      {
        Debug.WriteLine($"Resource not found: {sourceName}");
        return;
      }

      using var reader = new StreamReader(stream);
      reader.ReadLine();

      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        var values = line.Split(",");
        if (values.Length != 5)
        {
          Debug.WriteLine($"Invalid row format: {line}");
          continue;
        }
        context.Ingredients.Add(new Ingredient
        {
          FdcId = Convert.ToInt32(values[0]),
          Name = values[1],
          Description = values[2],
          Category = values[3],
          LastUpdated = Convert.ToDateTime(values[4])
        });
      }

      Debug.WriteLine("Ingredients seeded successfully.");
    }
    public static void SeedUserIngredients(KroContext context, Assembly assembly)
    {
      string sourceName = "KroApp.Server.Models.SeedData.UserIngredients.csv";

      using var stream = assembly.GetManifestResourceStream(sourceName);
      if (stream == null)
      {
        Debug.WriteLine($"Resource not found: {sourceName}");
        return;
      }

      using var reader = new StreamReader(stream);
      reader.ReadLine();

      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        var values = line.Split(",");
        if (values.Length != 8)
        {
          Debug.WriteLine($"Invalid row format: {line}");
          continue;
        }
        context.UserIngredients.Add(new UserIngredient
        {
          FdcId = Convert.ToInt32(values[0]),
          UserId = values[1],
          IngredientId = Convert.ToInt32(values[2]),
          DateAdded = DateOnly.Parse(values[3]),
          Quantity = Convert.ToDouble(values[4]),
          ExpirationDate = DateOnly.Parse(values[5]),
          Location = values[6],
          Notes = values[7]
        });
      }

      Debug.WriteLine("UserIngredients seeded successfully.");
    }
  }
}
