using System.ComponentModel.DataAnnotations;
using smartMonitoringBE.Domain.Entitities.Tiers;

namespace smartMonitoringBE.Models.DTO.Onboarding;

public sealed class CompleteOnboardingRequest
{
    [Required]
    [RegularExpression("personal|business", ErrorMessage = "accountType must be 'personal' or 'business'.")]
    public string AccountType { get; init; } = default!;

    [Required]
    
    public Guid PlanVersionId  { get; init; } = default!;

    public BusinessOnboardingDto? Business { get; init; }
    public PersonalOnboardingDto? Personal { get; init; }
}

public sealed class BusinessOnboardingDto
{
    [Required, MinLength(2)]
    public string BusinessName { get; init; } = default!;

    [Required]
    public string Industry { get; init; } = default!;

    [Required]
    public string Country { get; init; } = default!;

    [Required]
    public string AddressLine1 { get; init; } = default!;

    public string? AddressLine2 { get; init; }

    [Required]
    public string City { get; init; } = default!;

    [Required]
    public string Province { get; init; } = default!;

    [Required]
    public string PostalCode { get; init; } = default!;
}

public sealed class PersonalOnboardingDto
{
    [Required, MinLength(2)]
    public string PreferredName { get; init; } = default!;

    [Required]
    public string Country { get; init; } = default!;

    [Required]
    public string City { get; init; } = default!;

    [Required]
    public string Province { get; init; } = default!;

    [Required]
    public string PostalCode { get; init; } = default!;
}