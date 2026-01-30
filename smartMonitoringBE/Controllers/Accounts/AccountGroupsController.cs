using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO;
using smartMonitoringBE.Services.Account;

namespace smartMonitoringBE.Controllers;

[ApiController]
[Route("api/accounts/{accountId:guid}/groups")]
public sealed class AccountGroupsController : ControllerBase
{
    private readonly IAccountGroupService _svc;

    public AccountGroupsController(IAccountGroupService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public Task<List<AccountGroupListItemDto>> Get(Guid accountId, CancellationToken ct)
        => _svc.GetGroupsAsync(accountId, ct);

    [HttpGet("{groupId:guid}")]
    public Task<AccountGroupDto> GetOne(Guid accountId, Guid groupId, CancellationToken ct)
        => _svc.GetGroupAsync(accountId, groupId, ct);

    [HttpPost]
    public Task<AccountGroupDto> Create(Guid accountId, [FromBody] CreateGroupRequest req, CancellationToken ct)
        => _svc.CreateAsync(accountId, req, ct);

    [HttpPut("{groupId:guid}")]
    public Task<AccountGroupDto> Update(Guid accountId, Guid groupId, [FromBody] UpdateGroupRequest req, CancellationToken ct)
        => _svc.UpdateAsync(accountId, groupId, req, ct);

    [HttpPut("{groupId:guid}/scopes")]
    public Task<AccountGroupDto> ReplaceScopes(Guid accountId, Guid groupId, [FromBody] ReplaceGroupScopesRequest req, CancellationToken ct)
        => _svc.ReplaceScopesAsync(accountId, groupId, req, ct);

    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> Delete(Guid accountId, Guid groupId, CancellationToken ct)
    {
        await _svc.DeleteAsync(accountId, groupId, ct);
        return NoContent();
    }
}