using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Tiers;


public class PlanVersionConfig : IEntityTypeConfiguration<PlanVersion>
{
    public void Configure(EntityTypeBuilder<PlanVersion> b)
    {
        b.ToTable("PlanVersions");

        b.HasKey(x => x.Id);

        b.Property(x => x.Version)
            .IsRequired();

        b.Property(x => x.EntitlementsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        b.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        b.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.BillingInterval)
            .IsRequired();

        b.Property(x => x.IsTrial)
            .IsRequired();

        b.Property(x => x.TrialDays);

        b.Property(x => x.IsActive)
            .IsRequired();

        b.HasIndex(x => new { x.PlanId, x.Version })
            .IsUnique();

        b.HasMany(x => x.Accounts)
            .WithOne(a => a.PlanVersion)
            .HasForeignKey(a => a.PlanVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}