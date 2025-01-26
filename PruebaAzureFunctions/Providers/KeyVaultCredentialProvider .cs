using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using PruebaAzureFunctions.Interfaces;
using PruebaAzureFunctions.Results;
using System;
using System.Threading.Tasks;

namespace PruebaAzureFunctions.Providers
{
    public class KeyVaultCredentialProvider : IKeyVaultCredentialProvider
    {
        private readonly Lazy<Task<Result>> _smtpUsername;
        private readonly Lazy<Task<Result>> _smtpPassword;
        private string error = string.Empty;

        /// <summary>
        /// Constructor para inicializar los secretos desde Azure Key Vault.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación, que contiene la URL del KeyVault.</param>
        public KeyVaultCredentialProvider(IConfiguration configuration)
        {
            string Load_keyVaultUrl = string.Empty;
            string Load_smtp_username = string.Empty;
            string Load_smtp_password = string.Empty;
            try
            {
                Load_keyVaultUrl = configuration["Load_KeyVaultUrl"];
                Load_smtp_username = configuration["Load_smtp_username"];
                Load_smtp_password = configuration["Load_smtp_password"];
                if (string.IsNullOrEmpty(Load_keyVaultUrl)   ||
                    string.IsNullOrEmpty(Load_smtp_username) ||
                    string.IsNullOrEmpty(Load_smtp_password))
                {
                    error = "ERROR: Sin variables de configuración.";
                }
            }
            catch (Exception ex)
            {
                error = $"ERROR al leer KeyVaultUrl de configuración: {ex.Message}";
            }

            SecretClient secretClient = null;
            try
            {
                // Intentamos crear el cliente de KeyVault
                secretClient = new SecretClient(new Uri(Load_keyVaultUrl), new DefaultAzureCredential());
            }
            catch (AuthenticationFailedException ex)
            {
                error = $"ERROR de autenticación al conectar con Azure Key Vault: {ex.Message}";
            }
            catch (RequestFailedException ex)
            {
                error = $"ERROR al conectar con el Key Vault: {ex.Message}";
            }
            catch (Exception ex)
            {
                error = $"ERROR desconocido al crear el cliente de KeyVault: {ex.Message}";
            }

            // Usamos Lazy para cargar los secretos solo cuando se necesiten.
            _smtpUsername = new Lazy<Task<Result>>(() => LoadSecretAsync(secretClient, Load_smtp_username));
            _smtpPassword = new Lazy<Task<Result>>(() => LoadSecretAsync(secretClient, Load_smtp_password));
        }

        /// <summary>
        /// Carga un secreto desde Azure Key Vault.
        /// </summary>
        /// <param name="client">El cliente de Azure Key Vault.</param>
        /// <param name="secretName">El nombre del secreto a recuperar.</param>
        /// <returns>Resultado de la operación con éxito, mensaje y datos del secreto.</returns>
        private async Task<Result> LoadSecretAsync(SecretClient client, string secretName)
        {
            var result = new Result();

            if (!string.IsNullOrEmpty(error))
            {
                // Si ya existe un error, lo devolvemos en el resultado.
                result.isSuccess = false;
                result.message = error;
                result.data = null;
                return result;
            }

            try
            {
                // Recuperamos el secreto desde Key Vault
                var secret = await client.GetSecretAsync(secretName);
                result.isSuccess = true;
                result.message = "Se obtuvo el secreto exitosamente.";
                result.data = secret.Value.Value;
            }
            catch (RequestFailedException ex)
            {
                // Error específico de la solicitud a Key Vault
                result.isSuccess = false;
                result.message = $"ERROR al recuperar el secreto: {ex.Message}";
                result.data = null;
            }
            catch (Exception ex)
            {
                // Error general
                result.isSuccess = false;
                result.message = $"ERROR desconocido al recuperar el secreto: {ex.Message}";
                result.data = null;
            }

            return result;
        }

        /// <summary>
        /// Obtiene el nombre de usuario del servidor SMTP desde Key Vault.
        /// </summary>
        /// <returns>Resultado de la operación con éxito, mensaje y datos del nombre de usuario SMTP.</returns>
        public Task<Result> GetSmtpUsernameAsync() => _smtpUsername.Value;

        /// <summary>
        /// Obtiene la contraseña del servidor SMTP desde Key Vault.
        /// </summary>
        /// <returns>Resultado de la operación con éxito, mensaje y datos de la contraseña SMTP.</returns>
        public Task<Result> GetSmtpPasswordAsync() => _smtpPassword.Value;
    }

    
}
