namespace smartMonitoringBE.Models.DTO.ReferenceData;

public sealed record ReferenceListDto(IReadOnlyList<string> Items);

public sealed record CountryDto(string Code, string Name); // e.g. "ZA", "South Africa"
public sealed record CountryListDto(IReadOnlyList<CountryDto> Items);

public sealed record IndustryDto(string Code, string Name); // keep Code stable for DB later
public sealed record IndustryListDto(IReadOnlyList<IndustryDto> Items);