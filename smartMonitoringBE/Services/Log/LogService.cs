namespace smartMonitoringBE.services.log;

using System.Text.Json;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger;
    }

    public void Info(string message, object? props = null)
        => _logger.LogInformation("{Message} {Props}", message, ToJson(props));

    public void Warn(string message, object? props = null)
        => _logger.LogWarning("{Message} {Props}", message, ToJson(props));

    public void Error(string message, Exception? ex = null, object? props = null)
        => _logger.LogError(ex, "{Message} {Props}", message, ToJson(props));

    private static string? ToJson(object? props)
        => props == null ? null : JsonSerializer.Serialize(props);
}