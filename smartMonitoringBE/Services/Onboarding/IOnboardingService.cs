using System.Security.Claims;
using smartMonitoringBE.Models.DTO.Onboarding;

namespace smartMonitoringBE.Services.Onboarding;

public interface IOnboardingService
{
    Task CompleteAsync(ClaimsPrincipal user, CompleteOnboardingRequest req, CancellationToken ct);
}