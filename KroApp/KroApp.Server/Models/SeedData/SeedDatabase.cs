using KroApp.Server.Models.Ingredients;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace KroApp.Server.Models.SeedData
{
  public class SeedDatabase
  {
    public static void Initialize(IServiceProvider serviceProvider, Assembly assembly)
    {
      using var context = new KroContext(serviceProvider.GetRequiredService<DbContextOptions<KroContext>>());

      if (!context.Units.Any())
      {
        SeedUnits(context, assembly);
        context.SaveChanges();
      }

      context.SaveChanges();
    }

    public static void SeedUnits(KroContext context, Assembly assembly)
    {
      string sourceName = "KroApp.Server.Models.SeedData.Units.csv";

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
        if (values.Length != 4)
        {
          Debug.WriteLine($"Invalid row format: {line}");
          continue;
        }

        context.Units.Add(new Unit
        {
          Name = values[0],
          Abbreviation = values[1],
          ConversionFactorToBase = Convert.ToDouble(values[2]),
          UnitType = values[3],
        });
      }

      Debug.WriteLine("Units seeded successfully.");
    }

  }
}
