using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Security;

public sealed class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("Permissions");

        b.HasKey(x => x.Id);

        b.Property(x => x.Code)
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.Group)
            .HasMaxLength(60)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(300);

        b.Property(x => x.IsActive).IsRequired();

        b.HasIndex(x => x.Code).IsUnique();
    }
}