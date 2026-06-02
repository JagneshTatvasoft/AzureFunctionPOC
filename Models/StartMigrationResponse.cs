namespace AzureFunctionPOC.Models;

public sealed class StartMigrationResponse
{
    public Guid RunId { get; set; } = Guid.Empty;

    public DateTime StartedAtUtc { get; set; }
}