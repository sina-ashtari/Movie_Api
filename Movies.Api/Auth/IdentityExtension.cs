using System.Security.Claims;

namespace Movies.Api.Auth;

public static class IdentityExtension
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.SingleOrDefault(x => x.Type == "userid");
        if (Guid.TryParse(userId?.Value, out var parsedId))
        {
            return parsedId;
        }
        return null;
    }


}