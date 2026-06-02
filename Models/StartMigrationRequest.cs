namespace AzureFunctionPOC.Models;

public class StartMigrationRequest
{
    public Guid TenantId { get; set; }
    public short FilialNr { get; set; }

    public string ShirName { get; set; } = null!;

    public string ServerName { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
    // public string CustomerName { get; set; } = string.Empty;
}