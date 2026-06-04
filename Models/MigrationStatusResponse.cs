namespace AzureFunctionPOC.Models;
public class MigrationStatusResponse
{
    public Guid RunId { get; init; } = Guid.Empty;

    public string PipelineStatus { get; init; } = string.Empty;

    public DateTimeOffset? RunStartOn { get; init; }

    public DateTimeOffset? RunEndOn { get; init; }

    public string? Message { get; init; }
    public List<ActivityStatusResponse> Activities { get; set; } = [];
}