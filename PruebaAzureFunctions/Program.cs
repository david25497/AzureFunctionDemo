using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PruebaAzureFunctions.Interfaces;
using PruebaAzureFunctions.Providers;
using PruebaAzureFunctions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IKeyVaultCredentialProvider, KeyVaultCredentialProvider>();
    })
    .Build();

host.Run();