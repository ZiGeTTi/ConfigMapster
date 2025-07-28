using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class CustomLoggerPersistence<T>
{
    private static string Log(ILogger<T> exLogger, Exception ex, string message = null, string code = null)
    {
        var log = new
        {
            context = new Dictionary<string, string>
            {
                { "logger", exLogger.GetType().GenericTypeArguments.FirstOrDefault()?.FullName },
                { "message", ex?.Message ?? message },
                { "stackTrace", ex?.StackTrace ?? string.Empty },
                {
                    "environment",
                    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty
                },
                { "customMessage", message },
                { "errorCode", code }
            },
            message = ex?.Message ?? message
        };
        return JsonSerializer.Serialize(log);
    }

    public static void LogError(ILogger<T> exLogger, Exception ex, string message = null, string code = null) =>
        exLogger.LogError(Log(exLogger, ex, message, code));

    public static void LogCritical(ILogger<T> exLogger, Exception? ex, string? message = null, string? code = null) =>
        exLogger.LogCritical(Log(exLogger, ex, message, code));

    public static void LogWarning(ILogger<T> exLogger, Exception ex, string message = null, string code = null) =>
        exLogger.LogWarning(Log(exLogger, ex, message, code));

    public static void LogInformation(ILogger<T> exLogger, Exception ex = default, string message = null,
        string code = null) =>
        exLogger.LogInformation(Log(exLogger, ex, message, code));
}