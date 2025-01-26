using PruebaAzureFunctions.Results;

namespace PruebaAzureFunctions.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string toEmail, string subject, string body);
    }
}
