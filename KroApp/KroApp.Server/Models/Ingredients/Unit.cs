namespace KroApp.Server.Models.Ingredients
{
  public class Unit
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
    public double ConversionFactorToBase { get; set; }
    public required string UnitType { get; set; }
  }
}
