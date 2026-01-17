using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.User;


public sealed class AccountPlanChangeConfig : IEntityTypeConfiguration<AccountPlanChange>
{
    public void Configure(EntityTypeBuilder<AccountPlanChange> b)
    {
        b.ToTable("AccountPlanChanges");

        b.HasKey(x => x.Id);

        b.Property(x => x.Reason).HasMaxLength(300);

        b.HasIndex(x => x.AccountId);
        b.HasIndex(x => x.ChangedUtc);

        b.HasOne(x => x.Account)
            .WithMany() // or WithMany(a => a.PlanChanges) if you add a nav property
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.FromPlanVersion)
            .WithMany()
            .HasForeignKey(x => x.FromPlanVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.ToPlanVersion)
            .WithMany()
            .HasForeignKey(x => x.ToPlanVersionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasOne(x => x.Account)
            .WithMany(a => a.PlanChanges)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}