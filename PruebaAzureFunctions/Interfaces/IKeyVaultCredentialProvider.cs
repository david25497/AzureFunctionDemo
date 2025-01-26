using PruebaAzureFunctions.Results;

namespace PruebaAzureFunctions.Interfaces
{
    public interface IKeyVaultCredentialProvider
    {
        Task<Result> GetSmtpUsernameAsync();
        Task<Result> GetSmtpPasswordAsync();
    }

}
