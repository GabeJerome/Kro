using KroApp.Server.Controllers;
using KroApp.Server.Models.DTOs;
using KroApp.Server.Services.Ingredients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class IngredientsControllerTests
{
  private readonly Mock<IIngredientsService> _ingredientsServiceMock;
  private readonly IngredientsController _controller;

  public IngredientsControllerTests()
  {
    _ingredientsServiceMock = new Mock<IIngredientsService>();
    _controller = new IngredientsController(_ingredientsServiceMock.Object);
  }

  [Fact]
  public async Task Search_ReturnsOkResult_WithListOfIngredients()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "apple" };
    var expectedResults = new List<IngredientSearchResult>
        {
            new IngredientSearchResult { FdcId = 1, Name = "Apple" }
        };
    _ingredientsServiceMock.Setup(service => service.SearchIngredients(request))
        .ReturnsAsync(expectedResults);

    // Act
    var result = await _controller.Search(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = Assert.IsType<List<IngredientSearchResult>>(okResult.Value);
    Assert.Equal(expectedResults, returnValue);
  }

  [Fact]
  public async Task Search_CallsSearchIngredientsOnce()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "banana" };
    _ingredientsServiceMock.Setup(service => service.SearchIngredients(request))
        .ReturnsAsync(new List<IngredientSearchResult>());

    // Act
    await _controller.Search(request);

    // Assert
    _ingredientsServiceMock.Verify(service => service.SearchIngredients(request), Times.Once);
  }

  [Fact]
  public async Task Search_ReturnsBadRequest_WhenRequestIsNull()
  {
    // Act
    var result = await _controller.Search(null);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Invalid search request.", badRequestResult.Value);
  }

  [Fact]
  public async Task Search_ReturnsBadRequest_WhenQueryIsEmpty()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "" };

    // Act
    var result = await _controller.Search(request);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Invalid search request.", badRequestResult.Value);
  }

  [Fact]
  public async Task Search_ReturnsNotFound_WhenNoIngredientsFound()
  {
    // Arrange
    var request = new IngredientSearchRequest { Query = "unknown" };
    _ingredientsServiceMock.Setup(service => service.SearchIngredients(request))
        .ReturnsAsync(Enumerable.Empty<IngredientSearchResult>());

    // Act
    var result = await _controller.Search(request);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("No ingredients found.", notFoundResult.Value);
  }
}
