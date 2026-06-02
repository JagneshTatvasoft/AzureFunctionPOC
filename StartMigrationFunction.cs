using AzureFunctionPOC.Models;
using AzureFunctionPOC.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
// using Microsoft.AspNetCore.Http;



namespace Company.Function;

public class StartMigrationFunction(ILogger<StartMigrationFunction> logger, IMigrationService migrationService)
{
    [Function(nameof(StartMigrationFunction))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "migrations")] HttpRequestData req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var request =
           await req.ReadFromJsonAsync<StartMigrationRequest>();

        Guid runId =
            await migrationService.StartMigrationAsync(request!);

        var response =
           new StartMigrationResponse
           {
               RunId = runId,
               StartedAtUtc = DateTime.UtcNow
           };

        var httpResponse =
         req.CreateResponse(HttpStatusCode.OK);
         
        await httpResponse.WriteAsJsonAsync(response);

        return httpResponse;
    }
}