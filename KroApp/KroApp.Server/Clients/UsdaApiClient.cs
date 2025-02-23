using KroApp.Server.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace KroApp.Server.Clients
{
  public class UsdaApiClient : IUsdaApiClient
  {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;
    public UsdaApiClient(HttpClient httpClient, IConfiguration configuration)
    {
      _apiKey = configuration["UsdaApi:Key"] ?? throw new ArgumentNullException("UsdaApi:ApiKey configuration is missing");
      _httpClient = httpClient;
      _httpClient.BaseAddress = new Uri("https://api.nal.usda.gov/fdc/v1/foods/search");

      _jsonOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };
    }

    public async Task<IEnumerable<IngredientSearchResult>> Search(IngredientSearchRequest ingredientSearchBody)
    {
      if (string.IsNullOrWhiteSpace(ingredientSearchBody.Query))
      {
        return new List<IngredientSearchResult>();
      }

      var response = await _httpClient.PostAsJsonAsync($"?api_key={_apiKey}", ingredientSearchBody);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync();
      var searchResult = JsonSerializer.Deserialize<UsdaSearchResponse>(content, _jsonOptions);

      return searchResult?.Foods.Select(food => new IngredientSearchResult
      {
        FdcId = food.FdcId,
        Name = food.Description,
        Brand = !string.IsNullOrEmpty(food.BrandName) ? food.BrandName : food.BrandOwner,
        PackageWeight = food.ServingSize > 0
          ? $"{food.ServingSize} {food.ServingSizeUnit}"
          : "N/A",
        ServingSize = !string.IsNullOrEmpty(food.PackageWeight)
              ? food.PackageWeight
              : "N/A",
        Ingredients = food.Ingredients?.Split(", ") ?? new string[0],
        HighlightFields = food.HighlightFields
      }).ToList() ?? new List<IngredientSearchResult>();
    }
  }
}
