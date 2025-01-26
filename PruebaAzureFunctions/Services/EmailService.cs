using Azure.Security.KeyVault.Secrets;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using PruebaAzureFunctions.Interfaces;
using PruebaAzureFunctions.Results;  // Asegúrate de tener la clase Result

namespace PruebaAzureFunctions.Services
{
    public class EmailService : IEmailService
    {
        private readonly IKeyVaultCredentialProvider _credentialProvider;

        public EmailService(IKeyVaultCredentialProvider credentialProvider)
        {
            _credentialProvider = credentialProvider;
        }

        public async Task<Result> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Obtener las credenciales de SMTP usando Lazy
                var smtpUsernameResult = await _credentialProvider.GetSmtpUsernameAsync();
                var smtpPasswordResult = await _credentialProvider.GetSmtpPasswordAsync();

                // Validamos si obtuvimos las credenciales correctamente
                if (!smtpUsernameResult.isSuccess || !smtpPasswordResult.isSuccess)
                {
                    return new Result
                    {
                        isSuccess = false,
                        message = "Error de acceso: " + smtpUsernameResult.message
                    };
                }

                string smtpUsername = smtpUsernameResult.data.ToString();
                string smtpPassword = smtpPasswordResult.data.ToString();

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                // Verificar si el mensaje comienza con <html>
                bool isHtml = body.StartsWith("<html>", StringComparison.OrdinalIgnoreCase);

                // Agregar el mensaje adicional de advertencia solo si es HTML
                string footerMessage = "";
                if (isHtml)
                {
                    footerMessage = @"
                    <hr>
                    <p style='font-size: 12px; color: gray;'>Este es un correo de demostración, sin ninguna validez legal.</p>
                    <p style='font-size: 12px; color: gray;'>Cualquier uso indebido del contenido, por favor, responder a este correo para más información.</p>
                    <p style='font-size: 12px; color: gray;'>Gracias por su comprensión.</p> ";
                }
                else
                {
                    footerMessage = "\n\nEste es un correo de demostración, sin ninguna validez legal.\n" +
                                    "Cualquier uso indebido del contenido, por favor, responder a este correo para más información.\n" +
                                    "Gracias por su comprensión.";
                }

                // Concatenar el mensaje original con el pie de página
                body = body + footerMessage;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);

                return new Result
                {
                    isSuccess = true,
                    message = "Correo enviado exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    isSuccess = false,
                    message = "Error al enviar el correo: " + ex.Message
                };
            }
        }
    }
}
