using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data.User;

public class AccountUserConfig : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> b)
    {
        b.HasKey(x => new { x.AccountId, x.UserId });

        b.HasOne(x => x.Account)
            .WithMany(a => a.Users)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade); // fin

        b.HasOne(x => x.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction); 

        b.HasIndex(x => new { x.UserId, x.IsPrimary });
        
        b.Property(x => x.RoleId).IsRequired();

        b.HasOne(x => x.Role)
            .WithMany(r => r.AccountUsers)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}