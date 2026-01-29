using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.ENUMS;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Account;

namespace smartMonitoringBE.Services.Account;

public sealed class AccountProfileService : IAccountProfileService
{
    private readonly SmartMonitoringDbContext _db;

    public AccountProfileService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    public async Task<AccountProfileDto> GetAsync(Guid accountId, CancellationToken ct)
    {
        var acc = await _db.Accounts
            .AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => new AccountProfileDto
            {
                Id = a.Id,
                Name = a.Name,
                Industry = a.Industry,
                VatNumber = a.VatNumber,
                ContactEmail = a.ContactEmail,
                ContactPhone = a.ContactPhone,
                Address = a.Address == null ? null : new AddressDto
                {
                    Line1 = a.Address.Line1,
                    Line2 = a.Address.Line2,
                    City = a.Address.City,
                    Province = a.Address.Province,
                    PostalCode = a.Address.PostalCode,
                    Country = a.Address.Country
                }
            })
            .SingleOrDefaultAsync(ct);

        if (acc is null)
            throw new KeyNotFoundException("Account not found.");

        return acc;
    }

    public async Task<AccountProfileDto> PatchAsync(Guid accountId, UpdateAccountProfileRequest req, CancellationToken ct)
    {
        var acc = await _db.Accounts
            .SingleOrDefaultAsync(a => a.Id == accountId, ct);

        if (acc is null)
            throw new KeyNotFoundException("Account not found.");

        // Only update fields that were provided
        if (req.Name is not null)
            acc.Name = req.Name.Trim();

        if (req.Industry is not null)
            acc.Industry = string.IsNullOrWhiteSpace(req.Industry) ? null : req.Industry.Trim();

        if (req.VatNumber is not null)
            acc.VatNumber = string.IsNullOrWhiteSpace(req.VatNumber) ? null : req.VatNumber.Trim();

        if (req.ContactEmail is not null)
            acc.ContactEmail = string.IsNullOrWhiteSpace(req.ContactEmail) ? null : req.ContactEmail.Trim();

        if (req.ContactPhone is not null)
            acc.ContactPhone = string.IsNullOrWhiteSpace(req.ContactPhone) ? null : req.ContactPhone.Trim();

        if (req.Address is not null)
        {
            // Ensure owned entity exists
            acc.Address ??= new Address();

            // Patch address fields (null means "leave as-is" OR you can interpret as "clear". Choose one.)
            // Here: null => leave unchanged, empty string => clear
            acc.Address.Line1      = PatchString(acc.Address.Line1, req.Address.Line1);
            acc.Address.Line2      = PatchString(acc.Address.Line2, req.Address.Line2);
            acc.Address.City       = PatchString(acc.Address.City, req.Address.City);
            acc.Address.Province   = PatchString(acc.Address.Province, req.Address.Province);
            acc.Address.PostalCode = PatchString(acc.Address.PostalCode, req.Address.PostalCode);
            acc.Address.Country    = PatchString(acc.Address.Country, req.Address.Country);
        }

        acc.UpdatedUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        // Return fresh
        return await GetAsync(accountId, ct);
    }
    
     
    
    
    

    private static string? PatchString(string? current, string? incoming)
    {
        if (incoming is null) return current;   // not provided → leave unchanged
        return string.IsNullOrWhiteSpace(incoming) ? null : incoming.Trim();
    }
}