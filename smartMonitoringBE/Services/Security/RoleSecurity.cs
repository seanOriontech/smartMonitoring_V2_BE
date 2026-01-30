using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Security;
using smartMonitoringBE.Exceptions;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Security;

namespace smartMonitoringBE.Services.Security;

public sealed class RoleService : IRoleService
{
    private readonly SmartMonitoringDbContext _db;

    public RoleService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    public async Task<List<RoleListItemDto>> GetRolesAsync(Guid accountId, CancellationToken ct)
    {
        // system roles + account-owned roles
        var roles = await _db.Roles
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Where(r => (r.IsSystem && r.AccountId == null) || r.AccountId == accountId)
            .Select(r => new RoleListItemDto
            {
                Id = r.Id,
                AccountId = r.AccountId,
                Code = r.Code,
                Name = r.Name,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                PermissionCount = r.Permissions.Count
            })
            .OrderByDescending(r => r.IsSystem) // show system first
            .ThenBy(r => r.Name)
            .ToListAsync(ct);

        return roles;
    }

    public async Task<RoleDetailDto> GetRoleAsync(Guid accountId, Guid roleId, CancellationToken ct)
    {
        var role = await LoadRoleForAccountAsync(accountId, roleId, ct);

        return new RoleDetailDto
        {
            Id = role.Id,
            AccountId = role.AccountId,
            Code = role.Code,
            Name = role.Name,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = role.Permissions
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => new PermissionDto
                {
                    Id = rp.PermissionId,
                    Code = rp.Permission.Code,
                    Group = rp.Permission.Group,
                    Description = rp.Permission.Description
                })
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Code)
                .ToList()
        };
    }

    public async Task<RoleDetailDto> CreateRoleAsync(Guid accountId, CreateRoleRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Code))
            throw new ArgumentException("Role code is required.");
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Role name is required.");

        var code = NormaliseCode(req.Code);

        var exists = await _db.Roles.AnyAsync(r => r.AccountId == accountId && r.Code == code, ct);
        if (exists)
            throw new InvalidOperationException($"A role with code '{code}' already exists for this account.");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Code = code,
            Name = req.Name.Trim(),
            IsSystem = false,
            IsActive = true,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        if (req.PermissionIds is { Count: > 0 })
        {
            var perms = await _db.Permissions
                .Where(p => p.IsActive)
                .Where(p => req.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(ct);

            foreach (var pid in perms.Distinct())
                role.Permissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pid });
        }

        _db.Roles.Add(role);
        await _db.SaveChangesAsync(ct);

        return await GetRoleAsync(accountId, role.Id, ct);
    }

    public async Task<RoleDetailDto> PatchRoleAsync(Guid accountId, Guid roleId, PatchRoleRequest req, CancellationToken ct)
    {
        var role = await LoadRoleForAccountAsync(accountId, roleId, ct);

        if (role.IsSystem)
            throw new InvalidOperationException("System roles cannot be edited. Clone the role to customise it.");

        var changed = false;

        if (req.Name != null)
        {
            role.Name = req.Name.Trim();
            changed = true;
        }

        if (req.IsActive.HasValue)
        {
            role.IsActive = req.IsActive.Value;
            changed = true;
        }

        if (changed)
        {
            role.UpdatedUtc = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return await GetRoleAsync(accountId, roleId, ct);
    }

    public async Task<RoleDetailDto> ReplacePermissionsAsync(Guid accountId, Guid roleId, ReplaceRolePermissionsRequest req, CancellationToken ct)
    {
        var role = await LoadRoleForAccountAsync(accountId, roleId, ct);

        if (role.IsSystem)
            throw new InvalidOperationException("System roles cannot be edited. Clone the role to customise it.");

        var incoming = (req.PermissionIds ?? new List<Guid>())
            .Distinct()
            .ToHashSet();

        // validate permissions exist + active
        var valid = await _db.Permissions
            .Where(p => p.IsActive)
            .Where(p => incoming.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        var validSet = valid.ToHashSet();

        // remove missing
        role.Permissions = role.Permissions
            .Where(rp => validSet.Contains(rp.PermissionId))
            .ToList();

        // add new ones
        var existing = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();
        foreach (var pid in validSet)
        {
            if (!existing.Contains(pid))
                role.Permissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pid });
        }

        role.UpdatedUtc = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await GetRoleAsync(accountId, roleId, ct);
    }

    public async Task DeleteRoleAsync(Guid accountId, Guid roleId, CancellationToken ct)
    {
        var role = await LoadRoleForAccountAsync(accountId, roleId, ct);

        if (role.IsSystem)
            throw new InvalidOperationException("System roles cannot be deleted.");

        // block delete if assigned to any account user
        var inUse = await _db.AccountUsers
            .AnyAsync(au => au.AccountId == accountId && au.RoleId == roleId, ct);

        if (inUse)
            throw new ConflictException("This role is assigned to one or more users. Reassign users first.");

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync(ct);
    }

    // ----------------- helpers -----------------

    private static string NormaliseCode(string code)
        => code.Trim().ToLowerInvariant().Replace(' ', '-');

    private async Task<Role> LoadRoleForAccountAsync(Guid accountId, Guid roleId, CancellationToken ct)
    {
        var role = await _db.Roles
            .Include(r => r.Permissions)
                .ThenInclude(rp => rp.Permission)
            .SingleOrDefaultAsync(r => r.Id == roleId, ct);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        var ok = (role.IsSystem && role.AccountId == null) || role.AccountId == accountId;
        if (!ok)
            throw new InvalidOperationException("Role does not belong to this account.");

        return role;
    }
}