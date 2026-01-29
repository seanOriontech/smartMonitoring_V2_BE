using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data.User;



public class AccountConfig : IEntityTypeConfiguration<Account>
{
    
    public  readonly Guid DefaultPlanVersionId =
        Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111");
    
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.VatNumber).HasMaxLength(50);
        b.Property(x => x.ContactEmail).HasMaxLength(320);
        b.Property(x => x.ContactPhone).HasMaxLength(50);

        b.OwnsOne(x => x.Address);

        // If you store bytes, pick a sensible size; varbinary(max) by default is fine
       // b.Property(x => x.LogoContentType).HasMaxLength(100);
       // b.Property(x => x.LogoUrl).HasMaxLength(2048);
        
            b.Property(x => x.PlanVersionId)
            .IsRequired()
            .HasDefaultValue(DefaultPlanVersionId);
        b.HasOne(x => x.PlanVersion)
            .WithMany(v => v.Accounts)
            .HasForeignKey(x => x.PlanVersionId)
            .OnDelete(DeleteBehavior.Restrict); // don't delete plan versions when accounts are deleted
        
      
        
        
    }
}