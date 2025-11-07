using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using FluentAssertions;
using Moq;
using Services.Concrete;
using UnitTests.Common.Fixtures;
using Xunit;

namespace UnitTests.Services.Concrete;

public class ProjectServiceTests : IClassFixture<MapperFixture>
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IMapper _mapper;
    private readonly ProjectService _projectService;
    private readonly Guid _userId = Guid.NewGuid();

    public ProjectServiceTests(MapperFixture mapperFixture)
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userContextMock = new Mock<IUserContext>();
        _mapper = mapperFixture.Mapper;

        _userContextMock.Setup(x => x.UserId).Returns(_userId);
        _userContextMock.Setup(x => x.Email).Returns("admin@example.com");

        _projectService = new ProjectService(
            _projectRepositoryMock.Object,
            _userRepositoryMock.Object,
            _userContextMock.Object,
            _mapper
        );
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAsync_ShouldReturnNotFound_WhenNoProjectsExist()
    {
        _projectRepositoryMock
            .Setup(x => x.GetFromUserIdAsync(_userId))
            .ReturnsAsync(new List<Project>());

        var result = await _projectService.GetAsync();

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.ErrorMessage.Should().Be("No projects found for the current user.");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAsync_ShouldReturnOk_WhenProjectsExist()
    {
        var projects = new List<Project>
        {
            new Project
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProjectUsers = new List<ProjectUser>()
            }
        };

        _projectRepositoryMock
            .Setup(x => x.GetFromUserIdAsync(_userId))
            .ReturnsAsync(projects);

        var result = await _projectService.GetAsync();

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldCreateProjectWithCurrentUserAsAdmin()
    {
        var projectDto = new ProjectPostDto { ProjectName = "New Project" };
        var createdProject = new Project
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = projectDto.ProjectName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProjectUsers = new List<ProjectUser>
            {
                new ProjectUser { UserId = _userId, IsAdmin = true }
            }
        };

        _projectRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<Project>()))
            .ReturnsAsync(true);

        _projectRepositoryMock
            .Setup(x => x.GetFromUserIdAsync(_userId))
            .ReturnsAsync(new List<Project> { createdProject });

        var result = await _projectService.CreateAsync(projectDto);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.ProjectName.Should().Be(projectDto.ProjectName);

        _projectRepositoryMock.Verify(x => x.InsertAsync(It.Is<Project>(p =>
            p.ProjectUsers.Any(pu => pu.UserId == _userId && pu.IsAdmin)
        )), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddUserToProjectAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var projectId = Guid.NewGuid();
        var email = "nonexistent@example.com";

        var project = new Project
        {
            ProjectId = projectId,
            ProjectName = "Test Project",
            ProjectUsers = new List<ProjectUser>
            {
                new ProjectUser { UserId = _userId, IsAdmin = true }
            }
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);

        var result = await _projectService.AddUserToProjectAsync(projectId, email);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.ErrorMessage.Should().Be("User with this email not found.");
    }

    [Fact]
    public async System.Threading.Tasks.Task AddUserToProjectAsync_ShouldReturnBadRequest_WhenUserAlreadyInProject()
    {
        var projectId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        var email = "existing@example.com";

        var project = new Project
        {
            ProjectId = projectId,
            ProjectName = "Test Project",
            ProjectUsers = new List<ProjectUser>
            {
                new ProjectUser { UserId = _userId, IsAdmin = true },
                new ProjectUser { UserId = existingUserId, IsAdmin = false }
            }
        };

        var user = new User
        {
            UserId = existingUserId,
            Email = email,
            FirstName = "Existing",
            LastName = "User",
            Password = "hashedpassword"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        var result = await _projectService.AddUserToProjectAsync(projectId, email);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Be("User is already part of the project.");
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveUserFromProjectAsync_ShouldReturnBadRequest_WhenRemovingSelf()
    {
        var projectId = Guid.NewGuid();
        var email = "admin@example.com";

        var result = await _projectService.RemoveUserFromProjectAsync(projectId, email);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Be("You cannot remove yourself from the project.");
    }
}
