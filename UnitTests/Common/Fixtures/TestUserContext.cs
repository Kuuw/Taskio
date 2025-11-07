using Entities.Context.Abstract;

namespace UnitTests.Common.Fixtures;

public class TestUserContext : IUserContext
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = "test@example.com";
    public string FirstName { get; set; } = "Test";
    public string LastName { get; set; } = "User";
}
