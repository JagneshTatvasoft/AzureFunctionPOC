using System.Net;
using AzureFunctionPOC.Models;
using AzureFunctionPOC.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class GetMigrationStatusFunction(IMigrationService migrationService, ILogger<GetMigrationStatusFunction> logger)
{

    // [Function(nameof(GetMigrationStatusFunction))]
    // public async Task<HttpResponseData> Run(
    //     [HttpTrigger(
    //         AuthorizationLevel.Anonymous,
    //          "get",
    //         Route = "migrations/{runId}")]
    //     HttpRequestData req,
    //     string runId)
    // {
    //     // var query =
    //     //     System.Web.HttpUtility.ParseQueryString(
    //     //         req.Url.Query);

    //     // string runId =
    //     //     query["runId"] ?? string.Empty;

    //     var status =
    //         await migrationService
    //             .GetStatusAsync(runId);

    //     if (status == null)
    //     {
    //         return req.CreateResponse(
    //             HttpStatusCode.NotFound);
    //     }

    //     var response =
    //         req.CreateResponse(HttpStatusCode.OK);

    //     await response.WriteAsJsonAsync(status);

    //     return response;
    // }


 [Function(nameof(GetMigrationStatusFunction))]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "migrations/{runId}")]
        HttpRequestData req,
        Guid runId)
    {
        try
        {
            MigrationStatusResponse response =
                await migrationService.GetStatusAsync(runId);

            var httpResponse =
                req.CreateResponse(HttpStatusCode.OK);

            await httpResponse.WriteAsJsonAsync(response);

            return httpResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while retrieving migration status for RunId {RunId}",
                runId);

            var errorResponse =
                req.CreateResponse(
                    HttpStatusCode.InternalServerError);

            await errorResponse.WriteAsJsonAsync(
                new
                {
                    Error = "Failed to retrieve migration status",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });

            return errorResponse;
        }
    }
}