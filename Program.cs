using System.Text.Json;
using Azure.Monitor.OpenTelemetry.Exporter;
using AzureFunctionPOC.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<IMigrationService, ADFMigrationService>();

// builder.Services.AddOpenTelemetry()
//     .UseFunctionsWorkerDefaults()
//     .UseAzureMonitorExporter();

builder.Build().Run();
