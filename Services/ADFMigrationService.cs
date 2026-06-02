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

        var credential = new DefaultAzureCredential();

        var armClient = new ArmClient(
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

        var pipelineRun =
            await factory.Value.GetPipelineRunAsync(runId.ToString());

        return new MigrationStatusResponse
        {
            RunId = runId,
            Status = pipelineRun.Value.Status,
            RunStartOn = pipelineRun.Value.RunStartOn,
            RunEndOn = pipelineRun.Value.RunEndOn,
            Message = pipelineRun.Value.Message
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

        // var client =
        //     new DataFactoryManagementClient(
        //         subscriptionId,
        //         credential);

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