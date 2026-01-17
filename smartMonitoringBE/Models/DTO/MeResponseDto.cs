namespace smartMonitoringBE.Models.DTO;

public sealed record MeAccountDto(
    Guid AccountId,
    string AccountName,
    string AccountType,
    string Role,
    bool IsPrimary,
    bool IsDefault
);

public sealed record MeResponseDto(
    Guid UserId,
    string TenantId,
    string ObjectId,
    string? Email,
    string? GivenName,
    string? Surname,
    string? DisplayName,

    Guid PrimaryAccountId,
    bool RequiresOnboarding,

    IReadOnlyList<MeAccountDto> Accounts
);