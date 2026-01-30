using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.User.Group;

public sealed class AccountGroupConfig : IEntityTypeConfiguration<AccountGroup>
{
    public void Configure(EntityTypeBuilder<AccountGroup> b)
    {
        b.ToTable("AccountGroups");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.Property(x => x.Code).HasMaxLength(120);

        // Account -> Groups (choose NoAction to avoid multiple cascade paths)
        b.HasOne<smartMonitoringBE.Domain.Entitities.User.Account>()
            .WithMany() // add Account.Groups navigation later if you want
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        // Role assignment
        b.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        // Group -> Scopes
        b.HasMany(x => x.Scopes)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Group -> Users (join table)
        b.HasMany(x => x.Users)
            .WithOne(ug => ug.Group)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.AccountId, x.Name }).IsUnique(false);
    }
}