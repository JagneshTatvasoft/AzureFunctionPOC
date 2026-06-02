using AzureFunctionPOC.Models;

namespace AzureFunctionPOC.Services;

public interface IMigrationService
{
    // Task<string> StartMigrationAsync();

    Task<MigrationStatusResponse?> GetStatusAsync(Guid runId);
    
    public Task<Guid> StartMigrationAsync(
        StartMigrationRequest request);
}