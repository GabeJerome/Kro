namespace KroApp.Server.Models.DTOs
{
  public class IngredientSearchRequest
  {
    public required string Query { get; set; }
    public IEnumerable<string> DataType { get; set; } = new[] { "Foundation", "Branded", "SR Legacy", "Survey (FNDDS)" };
    public int PageSize { get; set; } = 25;
    public int PageNumber { get; set; } = 1;
    public string SortBy { get; set; } = "dataType.keyword";
    public string SortOrder { get; set; } = "asc";
    public string BrandOwner { get; set; } = string.Empty;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
  }
}
