using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Tiers;
using smartMonitoringBE.Services.Tiers;


[ApiController]
[Route("api/tier")]
public sealed class TierController : ControllerBase
{
    private readonly ITiersService _svc;

    public TierController(ITiersService svc) => _svc = svc;

    [HttpGet("latest")]
    public async Task<ActionResult<TierCatalogDto>> GetLatest(CancellationToken ct)
        => Ok(await _svc.GetLatestAsync(ct));
}