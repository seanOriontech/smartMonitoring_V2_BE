namespace smartMonitoringBE.Models.DTO;

public record MeResponseDto(
    Guid UserId,
    string TenantId,
    string ObjectId,
    string? Email,
    string? GivenName,
    string? Surname,
    string? DisplayName,
    Guid? AccountId,
    string? AccountName,
    string? AccountType,
    string? PlanTier,
    bool RequiresOnboarding
);