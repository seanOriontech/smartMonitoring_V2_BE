using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Security;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data.Security;

public sealed class AccountUserScopeConfig : IEntityTypeConfiguration<AccountUserScope>
{
    public void Configure(EntityTypeBuilder<AccountUserScope> b)
    {
        b.ToTable("AccountUserScopes");

        b.HasKey(x => x.Id);

        b.Property(x => x.TargetType).IsRequired();
        b.Property(x => x.IncludeDescendants).IsRequired();
        b.Property(x => x.CreatedUtc).IsRequired();

        // Prevent duplicate scopes
        b.HasIndex(x => new
            {
                x.AccountId,
                x.UserId,
                x.TargetType,
                x.WorkspaceId,
                x.WorkspaceNodeId
            })
            .IsUnique();

        // AccountUser (membership)
        // Cascade is safe here – deleting a membership removes its scopes
        b.HasOne(x => x.AccountUser)
            .WithMany(au => au.Scopes)
            .HasForeignKey(x => new { x.AccountId, x.UserId })
            .OnDelete(DeleteBehavior.Cascade);

        // Workspace
        // Do NOT cascade delete scopes when workspace is deleted
        b.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);

        // WorkspaceNode
        // Prevent deleting a node if scopes reference it
        b.HasOne(x => x.WorkspaceNode)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceNodeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}