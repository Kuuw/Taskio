using Entities.Models;

namespace Entities.DTO;

public partial class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Token { get; set; } = null!;

    public AuthenticateResponse()
    {
    }

    public AuthenticateResponse(User user, string token)
    {
        Id = user.UserId;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Token = token;
    }
}