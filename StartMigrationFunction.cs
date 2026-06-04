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
        try
        {
            var request = await req.ReadFromJsonAsync<StartMigrationRequest>();

            Guid runId = await migrationService.StartMigrationAsync(request!);

            var response = new StartMigrationResponse
            {
                RunId = runId,
                StartedAtUtc = DateTime.UtcNow
            };

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(response);

            return httpResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while starting the migration.");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            
            await errorResponse.WriteAsJsonAsync(new 
            { 
                Error = "Migration failed", 
                Message = ex.Message,
                StackTrace = ex.StackTrace // Note: Remove StackTrace in production for security
            });

            return errorResponse;
        }
    }
}