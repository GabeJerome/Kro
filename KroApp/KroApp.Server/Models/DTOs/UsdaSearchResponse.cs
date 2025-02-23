using System.Text.Json.Serialization;

namespace KroApp.Server.Models.DTOs
{
  public class UsdaSearchResponse
  {
    public int TotalHits { get; set; }
    public IEnumerable<UsdaFoodItem> Foods { get; set; } = Array.Empty<UsdaFoodItem>();

  }
}
