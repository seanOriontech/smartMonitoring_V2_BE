using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.Requests.Account;
using smartMonitoringBE.Services.Account;

namespace smartMonitoringBE.Controllers.Accounts;

[ApiController]
[Route("api/accounts/{accountId:guid}/theme")]
public sealed class AccountThemeController : ControllerBase
{
    private readonly IAccountThemeService _svc;
    public AccountThemeController(IAccountThemeService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<AccountThemeDto>> Get(Guid accountId, CancellationToken ct)
        => Ok(await _svc.GetThemeAsync(accountId, ct));

    [HttpPatch]
    public async Task<ActionResult<AccountThemeDto>> Patch(Guid accountId, [FromBody] PatchAccountThemeRequest req, CancellationToken ct)
        => Ok(await _svc.PatchThemeAsync(accountId, req, ct));
}