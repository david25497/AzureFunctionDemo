using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Xunit;
using PruebaAzureFunctions.DTOs.Requests;
using PruebaAzureFunctions.Interfaces;
using PruebaAzureFunctions.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using PruebaAzureFunctions.Functions;
using Microsoft.AspNetCore.Mvc;
using PruebaAzureFunctions.Providers;

namespace PruebaAzureFunctionsTest.Test
{
    public class FunctionMailTest : ConfigurationBase
    {       
        private readonly IEmailService _emailService;

        public FunctionMailTest()
        {
            // Resolver el servicio desde la clase base
            _emailService = GetService<IEmailService>();
        }

        [Fact]
        public async Task FunctionSendEmailTest()
        {
            // Preparar los datos de entrada para el correo
            var requestData = new RqMailDTO
            {
                toEmail = "droc.25497@gmail.com",
                subject = "Asunto: Email De Prueba",
                body = "Cuerpo: Prueba de integracion"
            };

            // Llamar al servicio directamente (simulando el comportamiento de la función)
            var sendResult = await _emailService.SendEmailAsync(requestData.toEmail, requestData.subject, requestData.body);

            // Verificar si el correo fue enviado correctamente
            Assert.True(sendResult.isSuccess, "El correo no se envió correctamente.");
        }
    }
}