namespace Entities.Context.Abstract;

public interface IUserContext
{
    Guid UserId { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
}