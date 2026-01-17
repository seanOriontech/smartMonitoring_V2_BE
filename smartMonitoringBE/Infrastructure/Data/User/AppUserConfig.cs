using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data.User;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
        b.Property(x => x.ObjectId).HasMaxLength(64).IsRequired();

        b.HasIndex(x => new { x.TenantId, x.ObjectId }).IsUnique();

        b.Property(x => x.Email).HasMaxLength(320);
        b.Property(x => x.GivenName).HasMaxLength(100);
        b.Property(x => x.Surname).HasMaxLength(100);
        b.Property(x => x.DisplayName).HasMaxLength(200);
        

    }
}