using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.User.Group;

namespace smartMonitoringBE.Infrastructure.Data.User.Group;

public sealed class AccountGroupScopeConfig : IEntityTypeConfiguration<AccountGroupScope>
{
    public void Configure(EntityTypeBuilder<AccountGroupScope> b)
    {
        b.ToTable("AccountGroupScopes");

        b.HasKey(x => x.Id);

        b.HasIndex(x => new { x.GroupId, x.WorkspaceId, x.WorkspaceNodeId })
            .IsUnique();

        b.HasOne(x => x.Group)
            .WithMany(g => g.Scopes)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.WorkspaceNode)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.IncludeDescendants)
            .HasDefaultValue(true);
    }
}