using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Infrastructure.Data.Structure;

public sealed class WorkspaceNodeConfig : IEntityTypeConfiguration<WorkspaceNode>
{
    public void Configure(EntityTypeBuilder<WorkspaceNode> b)
    {
        b.ToTable("WorkspaceNodes");

        b.HasKey(x => x.Id);

        // Required
        b.Property(x => x.WorkspaceId).IsRequired();
        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();

        // Optional display
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.Code).HasMaxLength(80);

        // UX / metadata
        b.Property(x => x.SortOrder).IsRequired();
      

        b.Property(x => x.TagsJson).HasMaxLength(8000);
        b.Property(x => x.MetadataJson).HasMaxLength(8000);

        // Location
        b.Property(x => x.Lat).HasPrecision(9, 6);
        b.Property(x => x.Lng).HasPrecision(9, 6);

        b.Property(x => x.TimeZone).HasMaxLength(64);

        // Lifecycle
        b.Property(x => x.IsActive).IsRequired();

        // Indexes
        b.HasIndex(x => x.WorkspaceId);
        b.HasIndex(x => x.ParentId);

        // Optional friendly code (unique per workspace when provided)
        b.HasIndex(x => new { x.WorkspaceId, x.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");

        // Workspace → Nodes
        // Prevent deleting workspace if nodes exist
        b.HasOne(x => x.Workspace)
            .WithMany(w => w.Nodes)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);

        // Self-reference (tree)
        // Prevent deleting a node that has children
        b.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Owned: Address
        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Line1).HasMaxLength(200);
            a.Property(p => p.Line2).HasMaxLength(200);
            a.Property(p => p.City).HasMaxLength(120);
            a.Property(p => p.Province).HasMaxLength(120);
            a.Property(p => p.PostalCode).HasMaxLength(30);
            a.Property(p => p.Country).HasMaxLength(2);
        });

        // Owned: Contact
        b.OwnsOne(x => x.Contact, c =>
        {
            c.Property(p => p.ContactName).HasMaxLength(200);
            c.Property(p => p.Phone).HasMaxLength(50);
            c.Property(p => p.Email).HasMaxLength(320);
        });
        
        b.HasOne(x => x.Workspace)
            .WithMany(w => w.Nodes)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}