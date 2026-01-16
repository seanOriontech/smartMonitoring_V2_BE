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
            .HasForeignKey(x => x.AccountId);

        b.HasOne(x => x.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(x => x.UserId);

        b.HasIndex(x => new { x.UserId, x.IsPrimary });
    }
}