using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Security;

public sealed class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");

        b.HasKey(x => x.Id);

        b.Property(x => x.Code)
            .HasMaxLength(80)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.IsSystem).IsRequired();
        b.Property(x => x.IsActive).IsRequired();

        // Unique per tenant (AccountId null = global/system)
        b.HasIndex(x => new { x.AccountId, x.Code }).IsUnique();
    }
}