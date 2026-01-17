using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Onboarding;
using Address = smartMonitoringBE.Domain.Entitities.User.Address;

namespace smartMonitoringBE.Services.Onboarding;

public sealed class OnboardingService : IOnboardingService
{
    private readonly SmartMonitoringDbContext _db;

    public OnboardingService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    public async Task CompleteAsync(ClaimsPrincipal principal, CompleteOnboardingRequest req, CancellationToken ct)
    {
        // Resolve identity (prefer OID; adjust if your claim names differ)
        var oid = principal.FindFirst("oid")?.Value;
       
        if (string.IsNullOrWhiteSpace(oid))
            throw new UnauthorizedAccessException("Missing oid claim.");

        // Load user + primary account link
        var user = await _db.Users
            .Include(u => u.Accounts)
                .ThenInclude(au => au.Account)
            .FirstOrDefaultAsync(u => u.ObjectId == oid , ct);

        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        var primaryLink = user.Accounts.FirstOrDefault(x => x.IsPrimary);
        if (primaryLink?.Account is null)
            throw new InvalidOperationException("Primary account not found for user.");

        var account = primaryLink.Account;

        // Map enums
        account.Type = MapAccountType(req.AccountType);
        account.PlanVersionId = req.PlanVersionId;

        var planVersion = await _db.PlanVersions.FindAsync(req.PlanVersionId);
        account.PlanStartedDateTime = DateTimeOffset.UtcNow;
        account.PlanVersionId = planVersion.Id;
        
        Workspace workspace = new Workspace();
        workspace.PrimaryWorkspace = true;
        workspace.CreatedUtc = DateTimeOffset.Now;
        workspace.TimeZone = "Africa/Johannesburg";

        workspace.Nodes = new List<WorkspaceNode>();
        WorkspaceNode newWorkSpaceNode = new WorkspaceNode();
        workspace.Nodes.Add(newWorkSpaceNode);
        newWorkSpaceNode.Name = "Home";
        newWorkSpaceNode.Type = WorkspaceNodeType.Folder;
     
  
     
        if (account.Type == AccountType.Business)
        {
            var b = req.Business;

            account.Name = b.BusinessName.Trim();
            account.Industry = b.Industry.Trim(); // add this field to Account if you don’t have it
            account.Address ??= new Address(); // adjust to your actual address type
            account.Address.Country = b.Country.Trim();
            account.Address.Line1 = b.AddressLine1.Trim();
            account.Address.Line2 = b.AddressLine2?.Trim();
            account.Address.City = b.City.Trim();
            account.Address.Province = b.Province.Trim();
            account.Address.PostalCode = b.PostalCode.Trim();
            
            workspace.Name =b.BusinessName.Trim();
            workspace.Type = WorkspaceType.Organisation;
            workspace.Code = await GenerateWorkspaceCodeAsync (account.Id, workspace.Name,ct);
          

        }
        else
        {
            var p = req.Personal;
            // Use preferred name on user + account display
            var preferred = p.PreferredName.Trim();
            user.DisplayName = preferred;
            account.Name = preferred;

            account.Address ??= new Address(); // adjust to your actual address type
            account.Address.Country = p.Country.Trim();
            account.Address.City = p.City.Trim();
            account.Address.Province = p.Province.Trim();
            account.Address.PostalCode = p.PostalCode.Trim();
            account.Address.Line1 ??= ""; // keep safe if your schema requires it
            
            workspace.Name =p.PreferredName.Trim();
            workspace.Type = WorkspaceType.Project;
            
            workspace.Code = await GenerateWorkspaceCodeAsync (account.Id, workspace.Name,ct);
        }

 

        // Optional: mark onboarding complete flag if you have one.
        // If your "requiresOnboarding" is computed from missing fields, then filling these will make it false.
        user.LastLoginUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
    
    public async Task<string> GenerateWorkspaceCodeAsync(
        Guid accountId,
        string name,
        CancellationToken ct)
    {
        var baseCode = SlugHelper.Slugify(name);
        var code = baseCode;
        var i = 1;

        while (await _db.Workspaces.AnyAsync(
                   w => w.AccountId == accountId && w.Code == code, ct))
        {
            i++;
            code = $"{baseCode}-{i}";
        }

        return code;
    }

    private static AccountType MapAccountType(string type) =>
        type.Trim().ToLowerInvariant() switch
        {
            "personal" => AccountType.Individual,
            "business" => AccountType.Business
           
        };

   
}