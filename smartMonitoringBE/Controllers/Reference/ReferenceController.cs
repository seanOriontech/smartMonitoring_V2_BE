using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Globalisation;
using smartMonitoringBE.Models.DTO.ReferenceData;

namespace smartMonitoringBE.Controllers.Reference;

[ApiController]
[Route("api/reference-data")]
[Authorize] // optional; remove if you want it public
public sealed class ReferenceDataController : ControllerBase
{
    private readonly IReferenceDataProvider _provider;

    public ReferenceDataController(IReferenceDataProvider provider)
    {
        _provider = provider;
    }

    [HttpGet("countries")]
    public ActionResult<CountryListDto> Countries()
        => Ok(_provider.GetCountries());

    [HttpGet("time-zones")]
    public ActionResult<ReferenceListDto> TimeZones()
        => Ok(_provider.GetTimeZones());

    [HttpGet("industries")]
    public ActionResult<IndustryListDto> Industries()
        => Ok(_provider.GetIndustries());
}