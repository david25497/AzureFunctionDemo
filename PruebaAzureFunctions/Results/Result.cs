namespace PruebaAzureFunctions.Results
{
    /// <summary>
    /// Clase que encapsula el resultado de una operación, incluyendo éxito, mensaje y datos.
    /// </summary>
    public class Result
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public object data { get; set; }  
    }
}
