namespace smartMonitoringBE.Security.Policies;

using Microsoft.AspNetCore.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionCode { get; }
    public PermissionRequirement(string code) => PermissionCode = code;
}