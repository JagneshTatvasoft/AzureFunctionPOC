namespace AzureFunctionPOC.Models;
public class MigrationStatus
{
    public string RunId { get; set; } = string.Empty;

    public int Progress { get; set; }

    public string Status { get; set; } = string.Empty;
}