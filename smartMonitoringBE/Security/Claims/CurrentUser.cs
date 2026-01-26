
using System.Security.Claims;

namespace smartMonitoringBE.Security.Claims;




public  class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public Guid UserId
    {
        get
        {
            var ctx = _http.HttpContext ?? throw new InvalidOperationException("No HttpContext");
            // B2C often uses "oid" for object id; adjust if you use "sub"
            var oid = ctx.User.FindFirstValue("oid") ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (oid is null) throw new UnauthorizedAccessException("Missing user id claim");
            return Guid.Parse(oid);
        }
    }
}