using Data.Abstract;
using Entities.DTO;
using Entities.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Services.Abstract;
using Services.Concrete;
using Xunit;

namespace UnitTests.Services.Concrete;

public class AuthServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IBcryptService> _bcryptServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _bcryptServiceMock = new Mock<IBcryptService>();

        _configMock.Setup(x => x["JwtSettings:Issuer"]).Returns("https://localhost:44373/");
        _configMock.Setup(x => x["JwtSettings:Audience"]).Returns("https://localhost:44373/");

        Environment.SetEnvironmentVariable("JWT_KEY", "SuperSecretKeyThatIsAtLeast32CharactersLong!");

        _authService = new AuthService(
            _configMock.Object,
            _userRepositoryMock.Object,
            _bcryptServiceMock.Object
        );
    }

    [Fact]
    public async System.Threading.Tasks.Task AuthenticateAsync_ShouldReturnBadRequest_WhenUserNotFound()
    {
        var userLogin = new UserLogin
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(userLogin.Email))
            .ReturnsAsync((User?)null);

        var result = await _authService.AuthenticateAsync(userLogin);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Be("Email or password is invalid.");
    }

    [Fact]
    public async System.Threading.Tasks.Task AuthenticateAsync_ShouldReturnBadRequest_WhenPasswordIsInvalid()
    {
        var userLogin = new UserLogin
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = userLogin.Email,
            FirstName = "John",
            LastName = "Doe",
            Password = "hashedpassword"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(userLogin.Email))
            .ReturnsAsync(user);

        _bcryptServiceMock
            .Setup(x => x.VerifyPassword(userLogin.Password, user.Password))
            .Returns(false);

        var result = await _authService.AuthenticateAsync(userLogin);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Be("Email or password is invalid.");
    }

    [Fact]
    public async System.Threading.Tasks.Task AuthenticateAsync_ShouldReturnOkWithToken_WhenCredentialsAreValid() // Change return type to Task
    {
        var userLogin = new UserLogin
        {
            Email = "user@example.com",
            Password = "correctpassword"
        };

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = userLogin.Email,
            FirstName = "John",
            LastName = "Doe",
            Password = "hashedpassword"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(userLogin.Email))
            .ReturnsAsync(user);

        _bcryptServiceMock
            .Setup(x => x.VerifyPassword(userLogin.Password, user.Password))
            .Returns(true);

        var result = await _authService.AuthenticateAsync(userLogin);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Should().NotBeNull();
    }
}
