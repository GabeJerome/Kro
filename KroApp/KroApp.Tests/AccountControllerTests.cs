using KroApp.Server.Controllers;
using KroApp.Server.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using KroApp.Server.Services;
using Microsoft.Extensions.Configuration;

public class AccountControllerTests
{
  private readonly Mock<UserManager<User>> _userManagerMock;
  private readonly Mock<SignInManager<User>> _signInManagerMock;
  private readonly Mock<IAuthService> _authServiceMock;
  private readonly Mock<IConfiguration> _configurationMock;
  private readonly AccountController _controller;

  public AccountControllerTests()
  {
    var userStoreMock = new Mock<IUserStore<User>>();
    _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

    var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    _signInManagerMock = new Mock<SignInManager<User>>(
        _userManagerMock.Object,
        httpContextAccessorMock.Object,
        Mock.Of<IUserClaimsPrincipalFactory<User>>(),
        null,
        Mock.Of<ILogger<SignInManager<User>>>(),
        Mock.Of<IAuthenticationSchemeProvider>(),
        Mock.Of<IUserConfirmation<User>>());

    _configurationMock = new Mock<IConfiguration>();
    _authServiceMock = new Mock<IAuthService>();

    _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _authServiceMock.Object);
  }

  private UserRegister ValidRegisterModel()
  {
    return new UserRegister
    {
      Username = "Test Username",
      Email = "testuser@example.com",
      Password = "Password123!",
      ConfirmPassword = "Password123!"
    };
  }

  private UserLogin ValidLoginModel()
  {
    return new UserLogin
    {
      Email = "testuser@example.com",
      Password = "Password123!",
      RememberMe = false
    };
  }

  // Test Cases for Register Action

  [Fact]
  public async Task Register_ReturnsSuccess_WhenModelIsValid()
  {
    // Arrange
    var expectedJwt = "mock-jwt";
    var registerModel = ValidRegisterModel();
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Success);
    _authServiceMock.Setup(auth => auth.GenerateJwtToken(registerModel.Email, false))
                    .Returns(expectedJwt);

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = okResult.Value;
    Assert.NotNull(response);

    var messageProperty = response.GetType().GetProperty("Message");
    var tokenProperty = response.GetType().GetProperty("Token");

    Assert.Equal("User registered successfully", messageProperty?.GetValue(response)?.ToString());
    Assert.Equal(expectedJwt, tokenProperty?.GetValue(response)?.ToString());
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    var existingUser = new User { UserName = registerModel.Email, Email = registerModel.Email };
    _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email))
                    .ReturnsAsync(existingUser);
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User already exists" }));

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("User already exists", (string[])errors[string.Empty]);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenPasswordIsInvalid()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    registerModel.Password = "123";
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("Password is too weak", (string[])errors[string.Empty]);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenEmailIsMissing()
  {
    // Arrange
    _controller.ModelState.AddModelError("Email", "Email is required");
    var registerModel = ValidRegisterModel();
    registerModel.Email = "";

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("Email is required", (string[])errors["Email"]);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenUsernameIsMissing()
  {
    // Arrange
    _controller.ModelState.AddModelError("Username", "Username is required");
    var registerModel = ValidRegisterModel();
    registerModel.Username = "";

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("Username is required", (string[])errors["Username"]);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }

  // Test Cases for Login Action

  [Fact]
  public async Task Login_ReturnsSuccess_WhenCredentialsAreValid()
  {
    // Arrange
    var loginModel = ValidLoginModel();
    var expectedJwt = "mock-jwt";
    _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                      .ReturnsAsync(IdentitySignInResult.Success);
    _authServiceMock.Setup(auth => auth.GenerateJwtToken(loginModel.Email, loginModel.RememberMe))
                    .Returns(expectedJwt);

    // Act
    var result = await _controller.Login(loginModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = okResult.Value;
    Assert.NotNull(response);

    var messageProperty = response.GetType().GetProperty("Message");
    var tokenProperty = response.GetType().GetProperty("Token");

    Assert.Equal("Login successful", messageProperty?.GetValue(response)?.ToString());
    Assert.Equal(expectedJwt, tokenProperty?.GetValue(response)?.ToString());
  }

  [Fact]
  public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
  {
    // Arrange
    var loginModel = ValidLoginModel();
    loginModel.Password = "wrongpassword"; // Invalid password
    _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                      .ReturnsAsync(IdentitySignInResult.Failed);

    // Act
    var result = await _controller.Login(loginModel);

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(unauthorizedResult.Value);
    Assert.Contains("Invalid login attempt.", (string[])errors[string.Empty]);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }

  [Fact]
  public async Task Login_ReturnsLockedOut_WhenUserIsLockedOut()
  {
    // Arrange
    var loginModel = ValidLoginModel();
    _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                      .ReturnsAsync(IdentitySignInResult.LockedOut);

    // Act
    var result = await _controller.Login(loginModel);

    // Assert
    var lockedResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(423, lockedResult.StatusCode);
    _authServiceMock.Verify(auth => auth.GenerateJwtToken(It.IsAny<string>(), false), Times.Never);
  }
}
