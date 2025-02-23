using KroApp.Server.Clients;
using KroApp.Server.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace KroApp.Tests.Clients
{
  public class UsdaApiClientTests
  {
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "test-api-key";
    private readonly UsdaApiClient _sut;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsdaApiClientTests()
    {
      _configurationMock = new Mock<IConfiguration>();
      _configurationMock.Setup(x => x["UsdaApi:Key"]).Returns(_apiKey);

      _handlerMock = new Mock<HttpMessageHandler>();
      _httpClient = new HttpClient(_handlerMock.Object);

      _sut = new UsdaApiClient(_httpClient, _configurationMock.Object);

      _jsonOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };
    }

    [Fact]
    public async Task Search_MinimalRequest_UsesDefaultValues()
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = "apple"
      };

      HttpRequestMessage? capturedRequest = null;

      _handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
          {
            capturedRequest = request;
          })
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new UsdaSearchResponse(), _jsonOptions))
          });

      // Act
      await _sut.Search(searchRequest);

      // Assert
      Assert.NotNull(capturedRequest);
      Assert.NotNull(capturedRequest.Content);
      var content = await capturedRequest.Content.ReadAsStringAsync();
      var requestBody = JsonSerializer.Deserialize<IngredientSearchRequest>(content, _jsonOptions);

      Assert.NotNull(requestBody);
      Assert.Equal(4, requestBody.DataType.Count());
      Assert.Contains("Branded", requestBody.DataType);
      Assert.Contains("Foundation", requestBody.DataType);
      Assert.Contains("SR Legacy", requestBody.DataType);
      Assert.Contains("Survey (FNDDS)", requestBody.DataType);
      Assert.Equal(25, requestBody.PageSize);
      Assert.Equal(1, requestBody.PageNumber);
      Assert.Equal("dataType.keyword", requestBody.SortBy);
      Assert.Equal("asc", requestBody.SortOrder);
      Assert.Equal("", requestBody.BrandOwner);
      Assert.Null(requestBody.StartDate);
      Assert.Null(requestBody.EndDate);
    }

    [Fact]
    public async Task Search_CustomDataType_OverridesDefault()
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = "apple",
        DataType = new[] { "Foundation", "SR Legacy" }
      };

      HttpRequestMessage? capturedRequest = null;

      _handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
          {
            capturedRequest = request;
          })
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new UsdaSearchResponse(), _jsonOptions))
          });

      // Act
      await _sut.Search(searchRequest);

      // Assert
      Assert.NotNull(capturedRequest);
      Assert.NotNull(capturedRequest.Content);
      var content = await capturedRequest.Content.ReadAsStringAsync();
      var requestBody = JsonSerializer.Deserialize<IngredientSearchRequest>(content, _jsonOptions);

      Assert.NotNull(requestBody);
      Assert.Equal(2, requestBody.DataType.Count());
      Assert.Contains("Foundation", requestBody.DataType);
      Assert.Contains("SR Legacy", requestBody.DataType);
    }

    [Fact]
    public async Task Search_FullResults_MapsCorrectly()
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = "apple",
        DataType = new[] { "Branded", "Foundation" }
      };

      var mockResponse = new UsdaSearchResponse
      {
        TotalHits = 100,
        Foods = new List<UsdaFoodItem>
                {
                    new UsdaFoodItem
                    {
                        FdcId = 12345,
                        Description = "Red Apple",
                        BrandName = "Nature's Best",
                        BrandOwner = "Nature Foods Inc",
                        ServingSize = 100,
                        ServingSizeUnit = "g",
                        PackageWeight = "1 kg",
                        Ingredients = "Organic apples",
                        HighlightFields = "description:Red Apple"
                    }
                }
      };

      var jsonResponse = JsonSerializer.Serialize(mockResponse, _jsonOptions);

      _handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
          });

      // Act
      var results = await _sut.Search(searchRequest);

      // Assert
      var result = Assert.Single(results);
      Assert.Equal(12345, result.FdcId);
      Assert.Equal("Red Apple", result.Name);
      Assert.Equal("Nature's Best", result.Brand);
      Assert.Equal("100 g", result.PackageWeight);
      Assert.Equal("1 kg", result.ServingSize);
      Assert.NotNull(result.Ingredients);
      Assert.Contains("Organic apples", result.Ingredients.First());
      Assert.Equal("description:Red Apple", result.HighlightFields);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task Search_InvalidQuery_ThrowsArgumentException(string? invalidQuery)
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = invalidQuery
      };

      // Act
      var result = await _sut.Search(searchRequest);

      // Assert
      Assert.Empty(result);
    }

    [Fact]
    public void Constructor_MissingApiKey_ThrowsArgumentNullException()
    {
      // Arrange
      var configMock = new Mock<IConfiguration>();
      configMock.Setup(x => x["UsdaApi:Key"]).Returns((string?)null);

      // Act & Assert
      Assert.Throws<ArgumentNullException>(() =>
          new UsdaApiClient(new HttpClient(), configMock.Object));
    }

    [Fact]
    public async Task Search_WithDateRange_SendsCorrectRequest()
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = "apple",
        StartDate = "2024-01-01",
        EndDate = "2024-02-01"
      };

      HttpRequestMessage? capturedRequest = null;

      _handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
          {
            capturedRequest = request;
          })
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new UsdaSearchResponse(), _jsonOptions))
          });

      // Act
      await _sut.Search(searchRequest);

      // Assert
      Assert.NotNull(capturedRequest);
      Assert.NotNull(capturedRequest.Content);
      var content = await capturedRequest.Content.ReadAsStringAsync();
      var requestBody = JsonSerializer.Deserialize<IngredientSearchRequest>(content, _jsonOptions);

      Assert.NotNull(requestBody);
      Assert.Equal("2024-01-01", requestBody.StartDate);
      Assert.Equal("2024-02-01", requestBody.EndDate);
    }

    [Fact]
    public async Task Search_InvalidResponse_ThrowsJsonException()
    {
      // Arrange
      var searchRequest = new IngredientSearchRequest
      {
        Query = "apple"
      };

      _handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("invalid json")
          });

      // Act & Assert
      await Assert.ThrowsAsync<JsonException>(() =>
          _sut.Search(searchRequest));
    }
  }
}
