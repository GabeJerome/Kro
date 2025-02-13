namespace KroApp.Server.Models.Ingredients
{
  public class Ingredient
  {
    public int Id { get; set; }
    public int FdcId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
  }
}
