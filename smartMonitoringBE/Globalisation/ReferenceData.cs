using smartMonitoringBE.Models.DTO.ReferenceData;

namespace smartMonitoringBE.Globalisation;

public interface IReferenceDataProvider
{
    CountryListDto GetCountries();
    ReferenceListDto GetTimeZones();
    IndustryListDto GetIndustries();
}

public sealed class ReferenceDataProvider : IReferenceDataProvider
{
    private static readonly CountryListDto Countries = new(new List<CountryDto>
    {
        new("ZA", "South Africa"),
        new("BW", "Botswana"),
        new("NA", "Namibia"),
        new("MZ", "Mozambique"),
        new("ZW", "Zimbabwe"),
        new("GB", "United Kingdom"),
        new("US", "United States"),
        // TODO: replace with full ISO list (recommended)
    });

    private static readonly IndustryListDto Industries = new(new List<IndustryDto>
    {
        new("manufacturing", "Manufacturing"),
        new("mining", "Mining & Metals"),
        new("utilities", "Utilities"),
        new("agriculture", "Agriculture"),
        new("logistics", "Logistics"),
        new("retail", "Retail"),
        new("healthcare", "Healthcare"),
        new("education", "Education"),
        new("technology", "Technology"),
        new("other", "Other"),
    });

    public CountryListDto GetCountries() => Countries;

    public ReferenceListDto GetTimeZones()
    {
        // NOTE: system timezones can differ on Linux vs Windows.
        var zones = TimeZoneInfo.GetSystemTimeZones()
            .Select(z => z.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ReferenceListDto(zones);
    }

    public IndustryListDto GetIndustries() => Industries;
}