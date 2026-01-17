using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using smartMonitoringBE.Domain.Entitities.Tiers;

public class PlanConfig : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> b)
    {
        b.ToTable("Plans");

        b.HasKey(x => x.Id);

        b.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.IsActive)
            .IsRequired();

        b.HasIndex(x => x.Code)
            .IsUnique();

        b.HasMany(x => x.Versions)
            .WithOne(v => v.Plan)
            .HasForeignKey(v => v.PlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}