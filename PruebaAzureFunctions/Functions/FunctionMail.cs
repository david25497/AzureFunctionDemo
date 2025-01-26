using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using PruebaAzureFunctions.DTOs.Requests;
using PruebaAzureFunctions.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace PruebaAzureFunctions.Functions
{
    public class FunctionMail
    {
        private readonly IEmailService _emailService;

        public FunctionMail(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [Function("FunctionSendEmail")]
        public async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
         FunctionContext context)
        {
            var logger = context.GetLogger("FunctionSendEmail");
            //Prueba de sincronización
            try
            {
                               
                // Deserializar directamente el cuerpo de la solicitud en el DTO
                var rqObject = await req.ReadFromJsonAsync<RqMailDTO>();

                // Validar el modelo
                if (rqObject == null)
                    return new BadRequestObjectResult("El cuerpo de la solicitud no tiene el formato correcto.");

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(rqObject);

                // Verificar las validaciones
                if (!Validator.TryValidateObject(rqObject, validationContext, validationResults, true))
                {
                    // Si no pasa la validación, devolver los errores de validación
                    return new BadRequestObjectResult(validationResults);
                }

                logger.LogInformation($"Enviando correo a {rqObject.toEmail} con asunto: {rqObject.subject}");

                var sendResult = await _emailService.SendEmailAsync(rqObject.toEmail, rqObject.subject, rqObject.body);
                bool send = sendResult.isSuccess;                

                if (!send)
                    return new BadRequestObjectResult("Ha ocurrido un error al enviar el email");

                return new OkObjectResult("Email enviado exitosamente");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error procesando la solicitud o enviando el correo");
                return new BadRequestObjectResult("El cuerpo de la solicitud no tiene un formato válido.");
            }
        }


    }

}
