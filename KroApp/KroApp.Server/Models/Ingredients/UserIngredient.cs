using KroApp.Server.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace KroApp.Server.Models.Ingredients
{
  public class UserIngredient
  {
    public int Id { get; set; }

    public int FdcId { get; set; }

    [ForeignKey("User")]
    public required string UserId { get; set; }

    [ForeignKey("Ingredient")]
    public int? IngredientId { get; set;}

    public DateOnly DateAdded { get; set; }

    public double Quantity { get; set; }

    public DateOnly? ExpirationDate { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }


    public User User { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
  }
}
