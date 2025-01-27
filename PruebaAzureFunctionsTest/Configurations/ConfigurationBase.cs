using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PruebaAzureFunctions.Interfaces;
using PruebaAzureFunctions.Providers;
using PruebaAzureFunctions.Services;

public abstract class ConfigurationBase : IDisposable
{
    protected readonly IHost _host;

    protected ConfigurationBase()
    {
        // Configuración del contenedor de DI
        _host = new HostBuilder()
            .ConfigureServices(services =>
            {
                // Cargar la configuración desde el archivo local.settings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
                    .Build();

                // Registrar la configuración en el contenedor
                services.AddSingleton<IConfiguration>(configuration);

                // Registrar las dependencias reales
                services.AddSingleton<IEmailService, EmailService>();
                services.AddSingleton<IKeyVaultCredentialProvider, KeyVaultCredentialProvider>();
            })
            .Build();
    }

    // Método para obtener servicios fácilmente
    protected T GetService<T>()
    {
        return _host.Services.GetRequiredService<T>();
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
}
