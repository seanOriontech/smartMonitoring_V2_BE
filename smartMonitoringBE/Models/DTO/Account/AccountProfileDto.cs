namespace smartMonitoringBE.Models.DTO.Account;

// Features/Accounts/Dtos/AccountProfileDto.cs
public sealed class AccountProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Industry { get; set; }

    public string? VatNumber { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public AddressDto? Address { get; set; }
}

public sealed class AddressDto
{
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}