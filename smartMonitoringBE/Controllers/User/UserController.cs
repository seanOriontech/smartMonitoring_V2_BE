using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.services.user;

namespace smartMonitoringBE.Controllers.User;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    
        private readonly IUserBootstrapService _svc;

        public UserController(IUserBootstrapService svc)
        {
            _svc = svc;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var dto = await _svc.GetOrCreateMeAsync(User, ct);
            return Ok(dto);
        }
    }
