using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data;

public class SmartMonitoringDbContext : DbContext
{
    public SmartMonitoringDbContext(DbContextOptions<SmartMonitoringDbContext> options)
        : base(options) { }

   
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartMonitoringDbContext).Assembly);
    }
    
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountUser> AccountUsers => Set<AccountUser>();
    
    
  
}