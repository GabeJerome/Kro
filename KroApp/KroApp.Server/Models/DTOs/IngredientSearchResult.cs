namespace KroApp.Server.Models.DTOs
{
  public class IngredientSearchResult
  {
    public required int FdcId { get; set; }
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public string? PackageWeight { get; set; }
    public string? ServingSize { get; set; }
    public IEnumerable<string>? Ingredients { get; set; }
    public string? HighlightFields { get; set; }
  }
}
