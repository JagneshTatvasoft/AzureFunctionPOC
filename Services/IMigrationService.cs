using AzureFunctionPOC.Models;

namespace AzureFunctionPOC.Services;

public interface IMigrationService
{
    // Task<string> StartMigrationAsync();

    Task<MigrationStatus?> GetStatusAsync(string runId);
    public Task<Guid> StartMigrationAsync(
        StartMigrationRequest request);
}