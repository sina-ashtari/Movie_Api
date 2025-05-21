using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Movies.Api.Auth;

public class AdminAuthRequirement : IAuthorizationRequirement, IAuthorizationHandler
{
    private readonly string _apiKey;

    public AdminAuthRequirement(string apiKey)
    {
        _apiKey = apiKey;
    }

    /// <summary>
    /// Handles JWT authentication or x-api-key authentication 
    /// </summary>
    /// 
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }
        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }
        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyHeaderValue))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        if (_apiKey != apiKeyHeaderValue)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("userId", Guid.NewGuid().ToString()));
        context.Succeed(this);
        return Task.CompletedTask;
    }
}