using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Security;

namespace smartMonitoringBE.Infrastructure.Data.Security;

public static class SecuritySeed
{
    // ----- Role IDs (global/system roles) -----
    public static readonly Guid RoleOwnerId    = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    public static readonly Guid RoleAdminId    = Guid.Parse("a1000000-0000-0000-0000-000000000002");
    public static readonly Guid RoleOperatorId = Guid.Parse("a1000000-0000-0000-0000-000000000003");
    public static readonly Guid RoleViewerId   = Guid.Parse("a1000000-0000-0000-0000-000000000004");
    public static readonly Guid RoleGuestId    = Guid.Parse("a1000000-0000-0000-0000-000000000005");

    // ----- Permission IDs -----
    public static readonly Guid PDevicesRead     = Guid.Parse("b2000000-0000-0000-0000-000000000001");
    public static readonly Guid PDevicesCreate   = Guid.Parse("b2000000-0000-0000-0000-000000000002");
    public static readonly Guid PDevicesUpdate   = Guid.Parse("b2000000-0000-0000-0000-000000000003");
    public static readonly Guid PDevicesDelete   = Guid.Parse("b2000000-0000-0000-0000-000000000004");
    public static readonly Guid PDevicesControl  = Guid.Parse("b2000000-0000-0000-0000-000000000005");

    public static readonly Guid PDataRead        = Guid.Parse("b2000000-0000-0000-0000-000000000010");
    public static readonly Guid PDataExport      = Guid.Parse("b2000000-0000-0000-0000-000000000011");

    public static readonly Guid PDashRead        = Guid.Parse("b2000000-0000-0000-0000-000000000020");
    public static readonly Guid PDashCreate      = Guid.Parse("b2000000-0000-0000-0000-000000000021");
    public static readonly Guid PDashUpdate      = Guid.Parse("b2000000-0000-0000-0000-000000000022");
    public static readonly Guid PDashDelete      = Guid.Parse("b2000000-0000-0000-0000-000000000023");
    public static readonly Guid PDashShare       = Guid.Parse("b2000000-0000-0000-0000-000000000024");

    public static readonly Guid PAlertsRead      = Guid.Parse("b2000000-0000-0000-0000-000000000030");
    public static readonly Guid PAlertsCreate    = Guid.Parse("b2000000-0000-0000-0000-000000000031");
    public static readonly Guid PAlertsUpdate    = Guid.Parse("b2000000-0000-0000-0000-000000000032");
    public static readonly Guid PAlertsDelete    = Guid.Parse("b2000000-0000-0000-0000-000000000033");
    public static readonly Guid PAlertsAck       = Guid.Parse("b2000000-0000-0000-0000-000000000034");
    public static readonly Guid PAlertsEscalate  = Guid.Parse("b2000000-0000-0000-0000-000000000035");

    public static readonly Guid PUsersRead       = Guid.Parse("b2000000-0000-0000-0000-000000000040");
    public static readonly Guid PUsersInvite     = Guid.Parse("b2000000-0000-0000-0000-000000000041");
    public static readonly Guid PUsersUpdateRole = Guid.Parse("b2000000-0000-0000-0000-000000000042");
    public static readonly Guid PUsersRemove     = Guid.Parse("b2000000-0000-0000-0000-000000000043");

    public static readonly Guid PBillingRead    = Guid.Parse("b2000000-0000-0000-0000-000000000050");
    public static readonly Guid PBillingManage  = Guid.Parse("b2000000-0000-0000-0000-000000000051");

    public static readonly Guid PAuditRead      = Guid.Parse("b2000000-0000-0000-0000-000000000060");

    public static void Seed(ModelBuilder mb)
    {
        // ---- Roles (global templates: AccountId = null) ----
        mb.Entity<Role>().HasData(
            new Role { Id = RoleOwnerId,    AccountId = null, Code = "owner",    Name = "Owner",    IsSystem = true, IsActive = true },
            new Role { Id = RoleAdminId,    AccountId = null, Code = "admin",    Name = "Admin",    IsSystem = true, IsActive = true },
            new Role { Id = RoleOperatorId, AccountId = null, Code = "operator", Name = "Operator", IsSystem = true, IsActive = true },
            new Role { Id = RoleViewerId,   AccountId = null, Code = "viewer",   Name = "Viewer",   IsSystem = true, IsActive = true },
            new Role { Id = RoleGuestId,    AccountId = null, Code = "guest",    Name = "Guest",    IsSystem = true, IsActive = true }
        );

        // ---- Permissions ----
        mb.Entity<Permission>().HasData(
            new Permission { Id = PDevicesRead,    Code = "devices.read",    Group = "Devices", Description = "View devices", IsActive = true },
            new Permission { Id = PDevicesCreate,  Code = "devices.create",  Group = "Devices", Description = "Create devices", IsActive = true },
            new Permission { Id = PDevicesUpdate,  Code = "devices.update",  Group = "Devices", Description = "Update devices", IsActive = true },
            new Permission { Id = PDevicesDelete,  Code = "devices.delete",  Group = "Devices", Description = "Delete devices", IsActive = true },
            new Permission { Id = PDevicesControl, Code = "devices.control", Group = "Devices", Description = "Send commands / control devices", IsActive = true },

            new Permission { Id = PDataRead,   Code = "data.read",   Group = "Data", Description = "View telemetry/data", IsActive = true },
            new Permission { Id = PDataExport, Code = "data.export", Group = "Data", Description = "Export telemetry/data", IsActive = true },

            new Permission { Id = PDashRead,   Code = "dashboards.read",   Group = "Dashboards", Description = "View dashboards", IsActive = true },
            new Permission { Id = PDashCreate, Code = "dashboards.create", Group = "Dashboards", Description = "Create dashboards", IsActive = true },
            new Permission { Id = PDashUpdate, Code = "dashboards.update", Group = "Dashboards", Description = "Edit dashboards", IsActive = true },
            new Permission { Id = PDashDelete, Code = "dashboards.delete", Group = "Dashboards", Description = "Delete dashboards", IsActive = true },
            new Permission { Id = PDashShare,  Code = "dashboards.share",  Group = "Dashboards", Description = "Share dashboards", IsActive = true },

            new Permission { Id = PAlertsRead,     Code = "alerts.read",        Group = "Alerts", Description = "View alerts/alarms", IsActive = true },
            new Permission { Id = PAlertsCreate,   Code = "alerts.create",      Group = "Alerts", Description = "Create alert rules", IsActive = true },
            new Permission { Id = PAlertsUpdate,   Code = "alerts.update",      Group = "Alerts", Description = "Edit alert rules", IsActive = true },
            new Permission { Id = PAlertsDelete,   Code = "alerts.delete",      Group = "Alerts", Description = "Delete alert rules", IsActive = true },
            new Permission { Id = PAlertsAck,      Code = "alerts.acknowledge", Group = "Alerts", Description = "Acknowledge alerts", IsActive = true },
            new Permission { Id = PAlertsEscalate, Code = "alerts.escalate",    Group = "Alerts", Description = "Escalate alerts", IsActive = true },

            new Permission { Id = PUsersRead,       Code = "users.read",        Group = "Users", Description = "View users/members", IsActive = true },
            new Permission { Id = PUsersInvite,     Code = "users.invite",      Group = "Users", Description = "Invite users", IsActive = true },
            new Permission { Id = PUsersUpdateRole, Code = "users.updateRole",  Group = "Users", Description = "Change user roles", IsActive = true },
            new Permission { Id = PUsersRemove,     Code = "users.remove",      Group = "Users", Description = "Remove users", IsActive = true },

            new Permission { Id = PBillingRead,   Code = "billing.read",   Group = "Billing", Description = "View billing/plan", IsActive = true },
            new Permission { Id = PBillingManage, Code = "billing.manage", Group = "Billing", Description = "Manage billing/plan", IsActive = true },

            new Permission { Id = PAuditRead, Code = "audit.read", Group = "Audit", Description = "View audit log", IsActive = true }
        );

        // ---- Role ↔ Permission mapping ----
        // Owner: everything
        AddRolePerms(mb, RoleOwnerId, AllPerms());

        // Admin: everything except billing.manage (you can decide)
        AddRolePerms(mb, RoleAdminId, AllPerms().Where(p => p != PBillingManage));

        // Operator: operational actions
        AddRolePerms(mb, RoleOperatorId, new[]
        {
            PDevicesRead, PDevicesUpdate, PDevicesControl,
            PDataRead,
            PDashRead, PDashUpdate,
            PAlertsRead, PAlertsAck, PAlertsEscalate
        });

        // Viewer: read-only
        AddRolePerms(mb, RoleViewerId, new[]
        {
            PDevicesRead, PDataRead,
            PDashRead,
            PAlertsRead,
            PAuditRead
        });

        // Guest: minimal read
        AddRolePerms(mb, RoleGuestId, new[]
        {
            PDashRead
        });
    }

    private static IEnumerable<Guid> AllPerms() => new[]
    {
        PDevicesRead, PDevicesCreate, PDevicesUpdate, PDevicesDelete, PDevicesControl,
        PDataRead, PDataExport,
        PDashRead, PDashCreate, PDashUpdate, PDashDelete, PDashShare,
        PAlertsRead, PAlertsCreate, PAlertsUpdate, PAlertsDelete, PAlertsAck, PAlertsEscalate,
        PUsersRead, PUsersInvite, PUsersUpdateRole, PUsersRemove,
        PBillingRead, PBillingManage,
        PAuditRead
    };

    private static void AddRolePerms(ModelBuilder mb, Guid roleId, IEnumerable<Guid> permIds)
    {
        mb.Entity<RolePermission>().HasData(
            permIds.Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            }).ToArray()
        );
    }
}