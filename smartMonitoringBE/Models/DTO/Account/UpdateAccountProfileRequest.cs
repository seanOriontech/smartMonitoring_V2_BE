namespace smartMonitoringBE.Models.DTO.Account;

public sealed class UpdateAccountProfileRequest
{
    public string? Name { get; set; }
    public string? Industry { get; set; }

    public string? VatNumber { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public AddressDto? Address { get; set; }
}