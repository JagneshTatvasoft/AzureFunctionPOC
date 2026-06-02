using System.Net;
using AzureFunctionPOC.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class GetMigrationStatusFunction(IMigrationService migrationService)
{

    [Function(nameof(GetMigrationStatusFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
             "get",
            Route = "migrations/{runId}")]
        HttpRequestData req,
        string runId)
    {
        // var query =
        //     System.Web.HttpUtility.ParseQueryString(
        //         req.Url.Query);

        // string runId =
        //     query["runId"] ?? string.Empty;

        var status =
            await migrationService
                .GetStatusAsync(runId);

        if (status == null)
        {
            return req.CreateResponse(
                HttpStatusCode.NotFound);
        }

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(status);

        return response;
    }
}