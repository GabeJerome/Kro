using KroApp.Server.Models.DTOs;

namespace KroApp.Server.Services.Ingredients
{
  public interface IIngredientsService
  {
    Task<IEnumerable<IngredientSearchResult>> SearchIngredients(IngredientSearchRequest body);
  }
}
