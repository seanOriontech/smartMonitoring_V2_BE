

using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Tiers;

namespace smartMonitoringBE.Infrastructure.Data.Tier;
public static class TierSeeder
{
public static void SeedPlans(ModelBuilder mb)
{
    mb.Entity<Plan>().HasData(
        new Plan { Id = PlanSeed.TrialPlanId,      Code = "trial",      Name = "Trial",      IsActive = true },
        new Plan { Id = PlanSeed.StarterPlanId,    Code = "starter",    Name = "Starter",    IsActive = true },
        new Plan { Id = PlanSeed.GrowthPlanId,     Code = "growth",     Name = "Growth",     IsActive = true },
        new Plan { Id = PlanSeed.BusinessPlanId,   Code = "business",   Name = "Business",   IsActive = true },
        new Plan { Id = PlanSeed.EnterprisePlanId, Code = "enterprise", Name = "Enterprise", IsActive = true }
    );

    mb.Entity<PlanVersion>().HasData(
        new PlanVersion
        {
            Id = PlanSeed.TrialV1Id,
            PlanId = PlanSeed.TrialPlanId,
            Version = 1,
            IsActive = true,
            IsTrial = true,
            TrialDays = 14,
            Currency = "ZAR",
            Price = 0m,
            BillingInterval = BillingInterval.None,
            EntitlementsJson =
            """
            {
              "features": { "alerts": true, "dashboards": true, "apiAccess": true , "whiteLabel" : true},
              "limits": { "maxDevices": 5, "maxUsers": 2, "maxDashboards": 3, "maxAlertRules": 10, "retentionDays": 7 }
            }
            """
        },
        new PlanVersion
        {
            Id = PlanSeed.StarterV1Id,
            PlanId = PlanSeed.StarterPlanId,
            Version = 1,
            IsActive = true,
            IsTrial = false,
            Currency = "ZAR",
            Price = 299m,
            BillingInterval = BillingInterval.Monthly,
            EntitlementsJson =
            """
            {
              "features": { "alerts": true, "dashboards": true, "apiAccess": true , "whiteLabel" : false},
              "limits": { "maxDevices": 25, "maxUsers": 5, "maxDashboards": 10, "maxAlertRules": 50, "retentionDays": 30 }
            }
            """
        },
        new PlanVersion
        {
            Id = PlanSeed.GrowthV1Id,
            PlanId = PlanSeed.GrowthPlanId,
            Version = 1,
            IsActive = true,
            IsTrial = false,
            Currency = "ZAR",
            Price = 899m,
            BillingInterval = BillingInterval.Monthly,
            EntitlementsJson =
            """
            {
              "features": { "alerts": true, "dashboards": true, "apiAccess": true, "deviceGroups": true , "whiteLabel" : false},
              "limits": { "maxDevices": 100, "maxUsers": 15, "maxDashboards": 50, "maxAlertRules": 250, "retentionDays": 90 }
            }
            """
        },
        new PlanVersion
        {
            Id = PlanSeed.BusinessV1Id,
            PlanId = PlanSeed.BusinessPlanId,
            Version = 1,
            IsActive = true,
            IsTrial = false,
            Currency = "ZAR",
            Price = 2499m,
            BillingInterval = BillingInterval.Monthly,
            EntitlementsJson =
            """
            {
              "features": { "alerts": true, "dashboards": true, "apiAccess": true, "deviceGroups": true, "smsNotifications": true , "whiteLabel" : true},
              "limits": { "maxDevices": 500, "maxUsers": 50, "maxDashboards": 200, "maxAlertRules": 2000, "retentionDays": 365 }
            }
            """
        },
        new PlanVersion
        {
            Id = PlanSeed.EnterpriseV1Id,
            PlanId = PlanSeed.EnterprisePlanId,
            Version = 1,
            IsActive = true,
            IsTrial = false,
            Currency = "ZAR",
            Price = 0m, // negotiated
            BillingInterval = BillingInterval.Monthly,
            EntitlementsJson =
            """
            {
              "features": { "alerts": true, "dashboards": true, "apiAccess": true, "deviceGroups": true, "smsNotifications": true, , "whiteLabel" : true },
              "limits": { "maxDevices": 999999, "maxUsers": 999999, "maxDashboards": 999999, "maxAlertRules": 999999, "retentionDays": 3650 }
            }
            """
        }
    );
}

}