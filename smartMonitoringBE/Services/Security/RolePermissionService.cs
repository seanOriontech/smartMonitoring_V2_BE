using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Security;

namespace smartMonitoringBE.Services.Security;

public sealed class RolePermissionService : IRolePermissionService
{
    private readonly SmartMonitoringDbContext _db;

    public RolePermissionService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    public async Task<RolePermissionsDto> GetRolePermissionsAsync(Guid accountId, Guid roleId, CancellationToken ct)
    {
        // Role must be system OR belong to this account
        var role = await _db.Roles
            .AsNoTracking()
            .Where(r => r.Id == roleId && (r.IsSystem || r.AccountId == accountId))
            .Select(r => new { r.Id, r.Name, r.IsSystem })
            .SingleOrDefaultAsync(ct);

        if (role is null)
            throw new InvalidOperationException("Role not found.");

        // All permissions (active)
        var perms = await _db.Permissions
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Group).ThenBy(p => p.Code)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                Group = p.Group,
                Description = p.Description
            })
            .ToListAsync(ct);

        // Selected permission codes for this role
        var selectedCodes = await _db.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission.Code)
            .ToListAsync(ct);

        return new RolePermissionsDto
        {
            RoleId = role.Id,
            RoleName = role.Name,
            IsSystem = role.IsSystem,
            Permissions = perms,
            SelectedCodes = selectedCodes
        };
    }

    public async Task<RolePermissionsDto> ReplaceRolePermissionsAsync(
        Guid accountId,
        Guid roleId,
        UpdateRolePermissionsRequest req,
        CancellationToken ct)
    {
        // Load role (tracked)
        var role = await _db.Roles
            .Where(r => r.Id == roleId && (r.IsSystem || r.AccountId == accountId))
            .SingleOrDefaultAsync(ct);

        if (role is null)
            throw new InvalidOperationException("Role not found.");

        // If later you want to lock system roles:
        // if (role.IsSystem) throw new InvalidOperationException("System roles cannot be edited.");

        // Normalise input
        var codes = (req.PermissionCodes ?? new List<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Resolve permission IDs
        var permissionIds = await _db.Permissions
            .AsNoTracking()
            .Where(p => p.IsActive && codes.Contains(p.Code))
            .Select(p => p.Id)
            .ToListAsync(ct);

        // Optional safety: fail if unknown codes sent
        if (permissionIds.Count != codes.Count)
            throw new InvalidOperationException("One or more permission codes are invalid.");

        // Remove existing links
        var existing = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(ct);

        _db.RolePermissions.RemoveRange(existing);

        // Add new links
        foreach (var pid in permissionIds)
        {
            _db.RolePermissions.Add(new Domain.Entitities.Security.RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });
        }

        role.UpdatedUtc = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        // Return updated view (same shape FE needs)
        return await GetRolePermissionsAsync(accountId, roleId, ct);
    }
}