using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartMonitoringBE.Domain.Entitities;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Infrastructure.Data.User;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.VatNumber).HasMaxLength(50);
        b.Property(x => x.ContactEmail).HasMaxLength(320);
        b.Property(x => x.ContactPhone).HasMaxLength(50);

        b.OwnsOne(x => x.Address);

        // If you store bytes, pick a sensible size; varbinary(max) by default is fine
        b.Property(x => x.LogoContentType).HasMaxLength(100);
        b.Property(x => x.LogoUrl).HasMaxLength(2048);
    }
}