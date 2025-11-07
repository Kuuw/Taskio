using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using FluentAssertions;
using Moq;
using Services.Abstract;
using Services.Concrete;
using UnitTests.Common.Fixtures;
using Xunit;

namespace UnitTests.Services.Concrete;

public class UserServiceTests : IClassFixture<MapperFixture>
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IBcryptService> _bcryptServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserServiceTests(MapperFixture mapperFixture)
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _bcryptServiceMock = new Mock<IBcryptService>();
        _userContextMock = new Mock<IUserContext>();
        _mapper = mapperFixture.Mapper;

        _userService = new UserService(
            _userRepositoryMock.Object,
            _bcryptServiceMock.Object,
            _userContextMock.Object,
            _mapper
        );
    }

    [Fact]
    public async System.Threading.Tasks.Task RegisterAsync_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        var userRegister = new UserRegister
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123!"
        };

        var existingUser = new User
        {
            UserId = Guid.NewGuid(),
            Email = userRegister.Email,
            FirstName = "Existing",
            LastName = "User",
            Password = "hashedpassword"
        };

        _userRepositoryMock
            .Setup(x => x.WhereAsync(It.IsAny<List<Func<User, bool>>>(), It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
            .ReturnsAsync(new List<User> { existingUser });

        var result = await _userService.RegisterAsync(userRegister);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Be("User with this email already exists.");
    }

    [Fact]
    public async System.Threading.Tasks.Task RegisterAsync_ShouldReturnOk_WhenRegistrationSucceeds()
    {
        var userRegister = new UserRegister
        {
            Email = "newuser@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "Password123!"
        };

        var hashedPassword = "hashed_password_123";
        var newUser = new User
        {
            UserId = Guid.NewGuid(),
            Email = userRegister.Email,
            FirstName = userRegister.FirstName,
            LastName = userRegister.LastName,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.WhereAsync(It.IsAny<List<Func<User, bool>>>(), It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
            .ReturnsAsync(new List<User>());

        _bcryptServiceMock
            .Setup(x => x.HashPassword(userRegister.Password))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<User>()))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(userRegister.Email))
            .ReturnsAsync(newUser);

        var result = await _userService.RegisterAsync(userRegister);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(userRegister.Email);
        result.Data.FirstName.Should().Be(userRegister.FirstName);
        result.Data.LastName.Should().Be(userRegister.LastName);

        _bcryptServiceMock.Verify(x => x.HashPassword(userRegister.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _userContextMock.Setup(x => x.UserId).Returns(userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var result = await _userService.GetAsync();

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.ErrorMessage.Should().Be("User not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAsync_ShouldReturnOk_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            UserId = userId,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "hashedpassword"
        };

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var result = await _userService.GetAsync();

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldReturnConflict_WhenEmailIsTaken()
    {
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            UserId = userId,
            Email = "current@example.com",
            FirstName = "Current",
            LastName = "User",
            Password = "hashedpassword"
        };

        var updateDto = new UserPutDto
        {
            Email = "taken@example.com",
            FirstName = "Updated",
            LastName = "User"
        };

        var conflictingUser = new User
        {
            UserId = Guid.NewGuid(),
            Email = "taken@example.com",
            FirstName = "Other",
            LastName = "User",
            Password = "hashedpassword"
        };

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.WhereAsync(It.IsAny<List<Func<User, bool>>>(), It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
            .ReturnsAsync(new List<User> { conflictingUser });

        var result = await _userService.UpdateAsync(updateDto);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Be("Email is already taken by another user");
    }
}
