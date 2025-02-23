using KroApp.Server.Models.DTOs;

namespace KroApp.Server.Services.Ingredients
{
  public class IngredientsService : IIngredientsService
  {
    private readonly IUsdaApiClient _usdaApiClient;

    public IngredientsService(IUsdaApiClient usdaApiClient)
    {
      _usdaApiClient = usdaApiClient;
    }

    public async Task<IEnumerable<IngredientSearchResult>> SearchIngredients(IngredientSearchRequest body)
    {
      return await _usdaApiClient.Search(body);
    }
  }
}
