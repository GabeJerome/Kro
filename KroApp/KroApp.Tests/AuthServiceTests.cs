using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using KroApp.Server.Models.Users;
using KroApp.Server.Services;
using Microsoft.AspNetCore.Http;

public class AuthServiceTests
{
  private readonly Mock<IConfiguration> _configurationMock;
  private readonly Mock<UserManager<User>> _userManagerMock;
  private readonly Mock<SignInManager<User>> _signInManagerMock;
  private readonly AuthService _authService;

  public AuthServiceTests()
  {
    _configurationMock = new Mock<IConfiguration>();
    _userManagerMock = new Mock<UserManager<User>>(
        Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

    _signInManagerMock = new Mock<SignInManager<User>>(
        _userManagerMock.Object,
        Mock.Of<IHttpContextAccessor>(),
        Mock.Of<IUserClaimsPrincipalFactory<User>>(),
        null,
        null,
        null,
        null);

    _authService = new AuthService(_configurationMock.Object, _userManagerMock.Object, _signInManagerMock.Object);
  }

  private UserRegister ValidRegisterModel() => new()
  {
    Username = "testuser",
    Email = "testuser@example.com",
    Password = "Password123!",
    ConfirmPassword = "Password123!"
  };

  private UserLogin ValidLoginModel() => new()
  {
    Username = "testuser",
    Password = "Password123!",
    RememberMe = true
  };

  [Fact]
  public void GenerateJwtToken_ReturnsValidToken()
  {
    // Arrange
    var email = "testuser@example.com";
    var username = "testuser";
    var rememberMe = true;
    _configurationMock.Setup(c => c["Jwt:Key"]).Returns("123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()=+");

    // Act
    var token = _authService.GenerateJwtToken(email, username, rememberMe);

    // Assert
    Assert.NotNull(token);
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    Assert.Equal(email, jwtToken.Subject);
    Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == username);
  }

  [Fact]
  public async Task LogIn_ReturnsSuccess_WhenCredentialsAreValid()
  {
    // Arrange
    var loginModel = ValidLoginModel();
    _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberMe, false))
                      .ReturnsAsync(SignInResult.Success);

    // Act
    var result = await _authService.LogIn(loginModel);

    // Assert
    Assert.True(result.Succeeded);
  }

  [Fact]
  public async Task RegisterUser_ReturnsSuccess_WhenUserCreated()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Success);

    // Act
    var result = await _authService.RegisterUser(registerModel);

    // Assert
    Assert.True(result.Succeeded);
  }

  [Fact]
  public async Task RegisterUser_ReturnsFailure_WhenPasswordIsWeak()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

    // Act
    var result = await _authService.RegisterUser(registerModel);

    // Assert
    Assert.False(result.Succeeded);
    Assert.Contains(result.Errors, e => e.Description == "Password is too weak");
  }

  [Fact]
  public async Task UserExists_ReturnsTrue_WhenUserExists()
  {
    // Arrange
    var username = "testuser";
    _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new User { UserName = username });

    // Act
    var exists = await _authService.UserExists(username);

    // Assert
    Assert.True(exists);
  }

  [Fact]
  public async Task UserExists_ReturnsFalse_WhenUserDoesNotExist()
  {
    // Arrange
    var username = "nonexistentuser";

    // Act
    var exists = await _authService.UserExists(username);

    // Assert
    Assert.False(exists);
  }

  [Fact]
  public async Task GetUser_ReturnsUser_WhenUserExists()
  {
    // Arrange
    var username = "testuser";
    var user = new User { UserName = username };
    _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(user);

    // Act
    var result = await _authService.GetUser(username);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(username, result.UserName);
  }

  [Fact]
  public async Task GetUser_ReturnsNull_WhenUserDoesNotExist()
  {
    // Arrange
    var username = "nonexistentuser";

    // Act
    var result = await _authService.GetUser(username);

    // Assert
    Assert.Null(result);
  }

}
