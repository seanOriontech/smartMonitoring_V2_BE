using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Infrastructure.Data.Structure;

public sealed class WorkspaceConfig : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> b)
    {
        b.ToTable("Workspaces");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);

        b.Property(x => x.Code).HasMaxLength(80).IsRequired();
        b.Property(x => x.Type).IsRequired();

        b.Property(x => x.TimeZone).HasMaxLength(64);

        b.Property(x => x.TagsJson).HasMaxLength(8000);
        b.Property(x => x.MetadataJson).HasMaxLength(8000);

        b.Property(x => x.IsActive).IsRequired();

        b.HasIndex(x => new { x.AccountId, x.Code }).IsUnique();
        b.HasIndex(x => x.AccountId);

   

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
   
        
        b.HasOne(x => x.Account)
            .WithMany(a => a.Workspaces)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction);
        
        
        
  
    }
    
    
}