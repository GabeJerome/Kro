using System.Text.Json.Serialization;

namespace KroApp.Server.Models.DTOs
{
  public class UsdaFoodItem
  {
    public int FdcId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string BrandOwner { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string ServingSizeUnit { get; set; } = string.Empty;
    public double ServingSize { get; set; }
    public string PackageWeight { get; set; } = string.Empty;
    public string? HighlightFields { get; set; } = string.Empty;
  }
}
