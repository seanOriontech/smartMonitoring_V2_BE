namespace smartMonitoringBE.Middleware;

public class AppException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }

    public AppException(int statusCode, string title, string message) : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }

    public static AppException BadRequest(string message) =>
        new(StatusCodes.Status400BadRequest, "Bad request", message);

    public static AppException NotFound(string message) =>
        new(StatusCodes.Status404NotFound, "Not found", message);

    public static AppException Forbidden(string message) =>
        new(StatusCodes.Status403Forbidden, "Forbidden", message);

    
}