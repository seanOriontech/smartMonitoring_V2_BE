using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.DTO.Branding;
using smartMonitoringBE.Services.Account;
using smartMonitoringBE.Services.Branding;


namespace smartMonitoringBE.Controllers;

[ApiController]
[Route("api/accounts/{accountId:guid}/branding")]
public class BrandingController : ControllerBase
{
    private readonly IAccountProfileService _svc;
    private readonly IAccountBrandingBlobService _abbs;
    private readonly IBrandingService _bs;

    public BrandingController(IAccountProfileService svc, IAccountBrandingBlobService abbs, IBrandingService bs)
    {
        _svc = svc;
        _abbs = abbs;
        _bs = bs;
    }
    
    [HttpGet]
    public async Task<ActionResult<BrandingDto>> GetBranding(
        Guid accountId,
        CancellationToken ct)
    {
        var dto = await _bs.GetBrandingAsync(accountId, ct);
        return Ok(dto);
    }

    /// <summary>
    /// Step 1: Ask backend for a SAS upload URL for a specific branding slot (LogoLight/LogoDark/etc).
    /// Frontend uploads the bytes directly to blob using the UploadUrl.
    /// </summary>
    [HttpPost("upload-url")]
    [ProducesResponseType(typeof(CreateBrandingUploadResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateBrandingUploadResponse>> CreateUploadUrl(
        Guid accountId,
        [FromBody] CreateBrandingUploadRequest req,
        CancellationToken ct)
    {
        var resp = await _abbs.CreateUploadAsync(accountId, req, ct);
        return Ok(resp);
    }

    /// <summary>
    /// Step 2: Commit the uploaded blob URL + metadata into the Account record.
    /// </summary>
    [HttpPatch]
    [ProducesResponseType(typeof(AccountProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AccountProfileDto>> PatchBranding(
        Guid accountId,
        [FromBody] PatchBrandingRequest req,
        CancellationToken ct)
    {
        var dto = await _bs.SetBrandingAssetAsync(accountId, req, ct);
        return Ok(dto);
    }
}