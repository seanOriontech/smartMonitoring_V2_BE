namespace smartMonitoringBE.Security;

public interface ICurrentUser
{
    Guid UserId { get; }
}