namespace smartMonitoringBE.Infrastructure.Data.Tier;

public static class PlanSeed
{
    // Plans
    public static readonly Guid TrialPlanId      = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid StarterPlanId    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid GrowthPlanId     = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid BusinessPlanId   = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid EnterprisePlanId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    // Versions
    public static readonly Guid TrialV1Id      = Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111");
    public static readonly Guid StarterV1Id    = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222");
    public static readonly Guid GrowthV1Id     = Guid.Parse("33333333-cccc-cccc-cccc-333333333333");
    public static readonly Guid BusinessV1Id   = Guid.Parse("44444444-dddd-dddd-dddd-444444444444");
    public static readonly Guid EnterpriseV1Id = Guid.Parse("55555555-eeee-eeee-eeee-555555555555");
}