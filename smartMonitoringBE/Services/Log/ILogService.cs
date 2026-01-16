namespace smartMonitoringBE.services.log;

public interface ILogService
{
    void Info(string message, object? props = null);
    void Warn(string message, object? props = null);
    void Error(string message, Exception? ex = null, object? props = null);
}