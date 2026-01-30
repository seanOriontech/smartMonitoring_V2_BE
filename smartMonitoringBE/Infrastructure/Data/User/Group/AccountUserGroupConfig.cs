using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.User.Group;

public sealed class AccountUserGroupConfig : IEntityTypeConfiguration<AccountUserGroup>
{
    public void Configure(EntityTypeBuilder<AccountUserGroup> b)
    {
        b.ToTable("AccountUserGroups");

        // v1: one group per user per account
        b.HasKey(x => new { x.AccountId, x.UserId });

        // ✅ FIX: prevent cascade path cycles
        b.HasOne(x => x.AccountUser)
            .WithMany()
            .HasForeignKey(x => new { x.AccountId, x.UserId })
            .OnDelete(DeleteBehavior.NoAction); // <-- IMPORTANT

        // ✅ OK to cascade when deleting the group
        b.HasOne(x => x.Group)
            .WithMany(g => g.Users)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.GroupId);
    }
}