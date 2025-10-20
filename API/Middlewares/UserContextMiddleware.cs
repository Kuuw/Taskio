using Entities.Context.Abstract;

namespace API.Middlewares;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        var user = context.User;

        var userIdClaim = user.FindFirst("UserId");
        var emailClaim = user.FindFirst("Email");
        var firstNameClaim = user.FindFirst("FirstName");
        var lastNameClaim = user.FindFirst("LastName");

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            userContext.UserId = userId;
        }

        userContext.Email = emailClaim?.Value ?? "";
        userContext.FirstName = firstNameClaim?.Value ?? "";
        userContext.LastName = lastNameClaim?.Value ?? "";

        await _next(context);
    }
}