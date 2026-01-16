using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Onboarding;
using smartMonitoringBE.Services.Onboarding;

namespace smartMonitoringBE.Controllers;

[ApiController]
[Route("api/onboarding")]
[Authorize]
public sealed class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _svc;

    public OnboardingController(IOnboardingService svc)
    {
        _svc = svc;
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromBody] CompleteOnboardingRequest req, CancellationToken ct)
    {
        await _svc.CompleteAsync(User, req, ct);
        return NoContent();
    }
}