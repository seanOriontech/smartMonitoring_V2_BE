using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.Security;
using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Domain.Entitities.Tiers;
using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.Infrastructure.Data.Tier;

namespace smartMonitoringBE.Infrastructure.Data;

public class SmartMonitoringDbContext : DbContext
{
    public SmartMonitoringDbContext(DbContextOptions<SmartMonitoringDbContext> options)
        : base(options) { }

   
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartMonitoringDbContext).Assembly);

        TierSeeder.SeedPlans(modelBuilder);
        
         Security.SecuritySeed.Seed(modelBuilder);
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountUser> AccountUsers => Set<AccountUser>();

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanVersion> PlanVersions => Set<PlanVersion>();
    
    public DbSet<AccountPlanChange> AccountPlanChanges => Set<AccountPlanChange>();
    
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    
    public DbSet<Workspace> Workspaces => Set<Workspace>();

    
  
}