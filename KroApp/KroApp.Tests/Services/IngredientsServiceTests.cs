using KroApp.Server.Models.DTOs;
using KroApp.Server.Services.Ingredients;
using Moq;
using Xunit;

public class IngredientsServiceTests
{
  private readonly Mock<IUsdaApiClient> _usdaApiClientMock;
  private readonly IngredientsService _ingredientsService;

  public IngredientsServiceTests()
  {
    _usdaApiClientMock = new Mock<IUsdaApiClient>();
    _ingredientsService = new IngredientsService(_usdaApiClientMock.Object);
  }

  [Fact]
  public async Task SearchIngredients_ShouldReturnResults()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "apple" };
    var expectedResults = new List<IngredientSearchResult>
          {
              new IngredientSearchResult { FdcId = 1, Name = "Apple" }
          };
    _usdaApiClientMock.Setup(client => client.Search(request)).ReturnsAsync(expectedResults);

    // Act
    var results = await _ingredientsService.SearchIngredients(request);

    // Assert
    Assert.Equal(expectedResults, results);
  }

  [Fact]
  public async Task SearchIngredients_ShouldCallUsdaApiClient()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "banana" };
    _usdaApiClientMock.Setup(client => client.Search(request)).ReturnsAsync(new List<IngredientSearchResult>());

    // Act
    await _ingredientsService.SearchIngredients(request);

    // Assert
    _usdaApiClientMock.Verify(client => client.Search(request), Times.Once);
  }
}
