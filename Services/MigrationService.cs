using System.Collections.Concurrent;
using AzureFunctionPOC.Models;

namespace AzureFunctionPOC.Services;

public class MigrationService 
{
    private static readonly ConcurrentDictionary<string, MigrationStatus>
        Jobs = new();

    public async Task<string> StartMigrationAsync()
    {
        var runId = Guid.NewGuid().ToString();

        Jobs[runId] = new MigrationStatus
        {
            RunId = runId,
            Progress = 0,
            Status = "Running"
        };

        _ = SimulatePipelineAsync(runId);

        return await Task.FromResult(runId);
    }


    private async Task SimulatePipelineAsync(string runId)
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(3000);

            if (Jobs.TryGetValue(runId, out var status))
            {
                status.Progress = i * 10;
            }
        }

        if (Jobs.TryGetValue(runId, out var completed))
        {
            completed.Status = "Succeeded";
        }
    }
}