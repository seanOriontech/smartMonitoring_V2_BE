using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Middleware;
using smartMonitoringBE.Models.DTO;
using smartMonitoringBE.Services.Auth;

namespace smartMonitoringBE.services.user;

public interface IUserBootstrapService
{
    Task<MeResponseDto> GetOrCreateMeAsync(ClaimsPrincipal user, CancellationToken ct);
}

public class UserBootstrapService : IUserBootstrapService
{
    private readonly SmartMonitoringDbContext _db;
    private readonly GraphMeService _graph;

    public UserBootstrapService(SmartMonitoringDbContext db, GraphMeService graph)
    {
        _db = db;
        _graph = graph;
    }

    public async Task<MeResponseDto> GetOrCreateMeAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        // ✅ Only use claims for stable identity keys + authorisation
        var tid = "";// principal.RequireClaim("tid");
       

        // ✅ Always use Graph for profile fields
        var me = await _graph.GetMeAsync(ct);
        
        try
        {
            me = await _graph.GetMeAsync(ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            // don’t leak raw Graph details to client
            throw new AppException(StatusCodes.Status503ServiceUnavailable, "Unable to load profile from Microsoft Graph.", ex.Message);
        }

        var given = me.GivenName?.Trim();
        var surname = me.Surname?.Trim();
        var display = me.DisplayName?.Trim();
        var oid = me.Id?.Trim();
        

        // Graph sometimes returns mail null -> fallback to UPN
        var email = (me.Mail ?? me.UserPrincipalName)?.Trim();

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var user = await _db.Users
                .Include(u => u.Accounts)
                .ThenInclude(au => au.Account)
                .Include( u=> u.Accounts)
                .ThenInclude( au=>au.Role)
                .FirstOrDefaultAsync(u =>  u.ObjectId == oid, ct);

            if (user is null)
            {
                user = new AppUser
                {
                    Id = new Guid(oid),
                    TenantId = tid,
                    ObjectId = oid,
                    Email = email,
                    GivenName = given,
                    Surname = surname,
                    DisplayName = display ?? $"{given} {surname}".Trim(),
                    LastLoginUtc = DateTimeOffset.UtcNow
                };
                _db.Users.Add(user);
            }
            else
            {

                user.LastLoginUtc = DateTimeOffset.UtcNow;
                user.Email = email;
                user.GivenName = given;
                user.Surname = surname;
                user.DisplayName = display ?? $"{given} {surname}".Trim();
            }

            // Ensure primary account link exists
            var primaryLink = user.Accounts?.FirstOrDefault(x => x.IsPrimary);

            Account account;

            if (primaryLink?.Account is not null)
            {
                account = primaryLink.Account;
            }
            else if (primaryLink is not null)
            {
                // link exists but nav not loaded for some reason
                account = await _db.Accounts.FirstAsync(a => a.Id == primaryLink.AccountId, ct);
            }
            else
            {
                account = new Account
                {
                    Id = Guid.NewGuid(),
                    //Type = AccountType.Individual,
                    // Tier = PlanTier.Starter,
                    Name = user.DisplayName ?? $"{given} {surname}".Trim(),
                    CreatedUtc = DateTimeOffset.UtcNow
                };
                _db.Accounts.Add(account);

                primaryLink = new AccountUser
                {
                    AccountId = account.Id,
                    UserId = user.Id,
                    //  Role = AccountRole.Owner,
                    IsPrimary = true,
                    JoinedUtc = DateTimeOffset.UtcNow,
                    IsDefault = true,
                    Account = account // ✅ set navigation so it’s available immediately

                };
                _db.AccountUsers.Add(primaryLink);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            var requiresOnboarding =
                (account.Type == AccountType.None || account.PlanVersionId == Guid.Empty);

            var accounts = user.Accounts

                .Where(au => au.Account != null)

                .Select(au => new MeAccountDto(
                    AccountId: au.AccountId,
                    AccountName: au.Account!.Name,
                    AccountType: au.Account!.Type.ToString(),
                    Role: au.Role.Name,
                    IsPrimary: au.IsPrimary,
                    IsDefault: au.IsDefault
                ))
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.IsPrimary)
                .ThenBy(a => a.AccountName)
                .ToList();

            return new MeResponseDto(
                UserId: user.Id,
                TenantId: user.TenantId,
                ObjectId: user.ObjectId,
                Email: user.Email,
                GivenName: user.GivenName,
                Surname: user.Surname,
                DisplayName: user.DisplayName,
                PrimaryAccountId: primaryLink.AccountId,
                Accounts: accounts,
                RequiresOnboarding: requiresOnboarding
            );
        }
        catch (OperationCanceledException)
        {
            
            throw;
        }
    catch (DbUpdateException ex) when (IsUniqueViolation(ex))
    {
        // Likely race condition: two requests tried to create same user/link.
        // You can optionally re-query and return success instead of conflict.
        //throw new AppException( S "Your account was created in another request. Please retry.");
        
        throw new AppException(StatusCodes.Status409Conflict, "Your account was created in another request. Please retry.", ex.Message);

    }
    catch (DbUpdateException ex)
    {
        throw new AppException(StatusCodes.Status500InternalServerError, "Unable to save your profile at this time.", ex.Message);
    }
}
    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        // SQL Server: 2601 (dup key), 2627 (unique constraint)
        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
            return sqlEx.Number is 2601 or 2627;

        return false;
    }
}