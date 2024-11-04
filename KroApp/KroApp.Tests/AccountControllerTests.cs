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

public class AccountControllerTests
{
  private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
  private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
  private readonly IAuthService _authService;
  private readonly AccountController _controller;

  public AccountControllerTests()
  {
    var userStoreMock = new Mock<IUserStore<IdentityUser>>();
    _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

    var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
        _userManagerMock.Object,
        httpContextAccessorMock.Object,
        Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
        null,
        Mock.Of<ILogger<SignInManager<IdentityUser>>>(),
        Mock.Of<IAuthenticationSchemeProvider>(),
        Mock.Of<IUserConfirmation<IdentityUser>>());

    _authService = new AuthService();

    _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _authService);
  }

  private UserRegister ValidRegisterModel()
  {
    return new UserRegister
    {
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
      Password = "Password123!"
    };
  }

  // Test Cases for Register Action

  [Fact]
  public async Task Register_ReturnsSuccess_WhenModelIsValid()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Success);

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    var existingUser = new IdentityUser { UserName = registerModel.Email, Email = registerModel.Email };
    _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email))
                    .ReturnsAsync(existingUser);
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User already exists" }));

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("User already exists", (string[])errors[string.Empty]);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenPasswordIsInvalid()
  {
    // Arrange
    var registerModel = ValidRegisterModel();
    registerModel.Password = "123"; // Weak password
    _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("Password is too weak", (string[])errors[string.Empty]);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
  {
    // Arrange
    _controller.ModelState.AddModelError("Email", "Email is required");
    var registerModel = new UserRegister { Email = "", Password = "Password123!", ConfirmPassword = "Password123!" };

    // Act
    var result = await _controller.Register(registerModel);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
    Assert.Contains("Email is required", (string[])errors["Email"]);
  }

  // Test Cases for Login Action

  [Fact]
  public async Task Login_ReturnsSuccess_WhenCredentialsAreValid()
  {
    // Arrange
    var loginModel = ValidLoginModel();
    _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                      .ReturnsAsync(IdentitySignInResult.Success);

    // Act
    var result = await _controller.Login(loginModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
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
  }
}
