using Entities.Models;

namespace Entities.DTO;

public partial class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Token { get; set; }

    public AuthenticateResponse(User user, string token)
    {
        Id = user.UserId;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Token = token;
    }
}