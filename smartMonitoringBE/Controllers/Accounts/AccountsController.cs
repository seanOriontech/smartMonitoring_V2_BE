// Features/Accounts/AccountsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Security.Policies;
using smartMonitoringBE.Services.Account;

[ApiController]
[Route("api/accounts/{accountId:guid}")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountProfileService _svc;

    public AccountsController(IAccountProfileService svc)
    {
        _svc = svc;
    }

    [HttpGet("profile")]
  //  [Authorize(Policy = PolicyNames.AccountsRead)]
    public async Task<IActionResult> GetProfile([FromRoute] Guid accountId, CancellationToken ct)
        => Ok(await _svc.GetAsync(accountId, ct));

    [HttpPatch("profile")]
  //  [Authorize(Policy = PolicyNames.AccountsUpdate)]
    public async Task<IActionResult> PatchProfile(
        [FromRoute] Guid accountId,
        [FromBody] UpdateAccountProfileRequest req,
        CancellationToken ct)
        => Ok(await _svc.PatchAsync(accountId, req, ct));
}