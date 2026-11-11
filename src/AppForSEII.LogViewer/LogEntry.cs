using System;

namespace AppForSEII.LogViewer;

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string LogLevel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }

}
