using KroApp.Server.Models.DTOs;

public interface IUsdaApiClient
{
  public Task<IEnumerable<IngredientSearchResult>> Search(IngredientSearchRequest ingredientSearchBody);
}