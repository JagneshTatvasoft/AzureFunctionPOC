public class ActivityStatusResponse
{
    public string Name { get; set; } = null!;
 
    public string Type { get; set; } = null!;
 
    public string Status { get; set; } = null!;
 
    public DateTimeOffset? StartedAt { get; set; }
 
    public DateTimeOffset? FinishedAt { get; set; }
 
    public long? DurationMs { get; set; }
 
    public string? Error { get; set; }
}