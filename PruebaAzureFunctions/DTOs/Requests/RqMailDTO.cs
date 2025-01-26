using System.ComponentModel.DataAnnotations;

namespace PruebaAzureFunctions.DTOs.Requests
{
    public class RqMailDTO
    {
        [Required(ErrorMessage = "El campo 'toEmail' es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo 'toEmail' debe ser una dirección de correo electrónico válida.")]
        public string toEmail { get; set; }

        [Required(ErrorMessage = "El campo 'subject' es obligatorio.")]
        public string subject { get; set; }

        [Required(ErrorMessage = "El campo 'body' es obligatorio.")]
        public string body { get; set; }
    }
}
