using System.Security.Claims;

namespace smartMonitoringBE.Services.Auth;

public static class ClaimsUserContext
{
    public static string? GetClaim(this ClaimsPrincipal user, string type)
        => user.FindFirst(type)?.Value;

    public static string RequireClaim(this ClaimsPrincipal user, string type)
        => user.GetClaim(type) ?? throw new UnauthorizedAccessException($"Missing claim: {type}");

    public static string? GetEmail(this ClaimsPrincipal user)
        => user.GetClaim("email")
           ?? user.GetClaim(ClaimTypes.Email)
           ?? user.GetClaim("preferred_username"); // fallback (not always email in CIAM)
}