using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.DataFactory;
using Azure.ResourceManager.DataFactory.Models;
using AzureFunctionPOC.Models;
using AzureFunctionPOC.Services;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionPOC.Services;

public sealed class ADFMigrationService(IConfiguration configuration) : IMigrationService
{
    public async Task<MigrationStatusResponse?> GetStatusAsync(Guid runId)
    {
        string subscriptionId =
           configuration["SubscriptionId"]!;

        string resourceGroup =
            configuration["ResourceGroup"]!;

        string factoryName =
            configuration["DataFactoryName"]!;

        string pipelineName =
            configuration["PipelineName"]!;

        DefaultAzureCredential credential = new DefaultAzureCredential();

        ArmClient armClient = new ArmClient(
            credential,
            subscriptionId);

        ResourceIdentifier factoryId =
            DataFactoryResource.CreateResourceIdentifier(
                subscriptionId,
                resourceGroup,
                factoryName);

        DataFactoryResource factory =
            await armClient
                .GetDataFactoryResource(factoryId)
                .GetAsync();

        // 1. Get the main pipeline run status
        DataFactoryPipelineRunInfo pipelineRun = await factory.GetPipelineRunAsync(runId.ToString());

        DateTimeOffset start = (pipelineRun.RunStartOn ?? DateTimeOffset.UtcNow).AddHours(-1);
        DateTimeOffset end = DateTimeOffset.UtcNow.AddHours(1);

        RunFilterContent filter = new RunFilterContent(start, end);

        AsyncPageable<PipelineActivityRunInformation> activityRunsPageable = factory.GetActivityRunAsync(
                  runId: runId.ToString(),
                  content: filter
              );

        List<ActivityStatusResponse> activitiesList = new List<ActivityStatusResponse>();

        await foreach (var activityRun in activityRunsPageable)
        {
            activitiesList.Add(new ActivityStatusResponse
            {
                Name = activityRun.ActivityName,
                Type = activityRun.ActivityType,
                Status = activityRun.Status,
                StartedAt = activityRun.StartOn,
                FinishedAt = activityRun.EndOn,
                DurationMs = activityRun.DurationInMs,
                Error = activityRun.Error?.ToString() 
            });
        }

        // 5. Return the newly structured response
        return new MigrationStatusResponse
        {
            RunId = runId,
            PipelineStatus = pipelineRun.Status, 
            RunStartOn = pipelineRun.RunStartOn,
            RunEndOn = pipelineRun.RunEndOn,    
            Message = pipelineRun.Message,
            Activities = activitiesList
        };
    }

    public async Task<Guid> StartMigrationAsync(
        StartMigrationRequest request)
    {
        string subscriptionId =
            configuration["SubscriptionId"]!;

        string resourceGroup =
            configuration["ResourceGroup"]!;

        string factoryName =
            configuration["DataFactoryName"]!;

        string pipelineName =
            configuration["PipelineName"]!;

        var credential =
            new DefaultAzureCredential();

        var armClient =
            new ArmClient(
                credential,
                subscriptionId);

        var factoryId =
  DataFactoryResource.CreateResourceIdentifier(
      subscriptionId,
      resourceGroup,
      factoryName);

        var factory =
             await armClient
                 .GetDataFactoryResource(factoryId)
                 .GetAsync();

        var pipeline =
await factory.Value
.GetDataFactoryPipelineAsync(
   pipelineName);

        var parameters = new Dictionary<string, BinaryData>
        {
            ["TenantId"] =
                BinaryData.FromObjectAsJson(request.TenantId.ToString()),

            ["FilialNr"] =
                BinaryData.FromObjectAsJson(request.FilialNr.ToString()),

            ["ShirName"] =
                BinaryData.FromObjectAsJson(request.ShirName),

            ["ServerName"] =
                BinaryData.FromObjectAsJson(request.ServerName),

            ["DatabaseName"] =
                BinaryData.FromObjectAsJson(request.DatabaseName),

            ["UserName"] =
                BinaryData.FromObjectAsJson(request.UserName),

            ["Password"] =
                BinaryData.FromObjectAsJson(request.Password)
        };

        var run = await pipeline.Value.CreateRunAsync(parameters);

        return run.Value.RunId;
    }



}