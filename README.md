# PruebaAzureFunctions

Este proyecto implementa Azure Functions y está diseñado para enviar correos electrónicos utilizando Gmail a través de `SmtpClient`. Las claves sensibles se manejan mediante Azure Key Vault con IAM (Identity Access Management) para garantizar la seguridad.

## Descripción

PruebaAzureFunctions es una solución basada en Azure Functions alojada bajo el Consumption Plan. Está configurada para realizar las siguientes tareas principales:

- **Envío de correos electrónicos**: Utilizando `SmtpClient` y credenciales almacenadas en Azure Key Vault.
- **Gestión segura de claves**: Integración con Azure Key Vault mediante IAM.
- **Ejecución automatizada**: Configuración de un pipeline en Azure DevOps para compilar y desplegar automáticamente en cada commit.

El proyecto sigue una estructura modular con separación clara de responsabilidades entre las funciones, servicios y proveedores.

## Estructura del Proyecto

La solución tiene la siguiente estructura:

```
PruebaAzureFunctions
├── Connected Services
├── Dependencias
├── Properties
├── DTOs
│   └── Requests
│       └── RqMailDTO.cs
├── Functions
│   ├── FunctionMail.cs
│   └── FunctionPrueba.cs
├── Interfaces
│   ├── IEmailService.cs
│   └── IKeyVaultCredentialProvider.cs
├── Providers
│   └── KeyVaultCredentialProvider.cs
├── Results
│   └── Result.cs
├── Services
│   └── EmailService.cs
├── host.json
├── local.settings.json (excluido del repositorio)
└── Program.cs

PruebaAzureFunctionsTest
├── Dependencias
├── Configurations
│   └── ConfigurationBase.cs
├── Tests
│   └── FunctionMailTest.cs
└── local.settings.json (excluido del repositorio)
```

## Configuración Inicial

### 1. Configurar el archivo `local.settings.json`

Este archivo **no está incluido en el repositorio**. Es necesario configurarlo manualmente con los valores apropiados. A continuación, un ejemplo de los campos requeridos:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<tu_conexión_de_storage>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "Load_KeyVaultUrl": "https://TuURL.vault.azure.net/",
  "Load_smtp_username": "username",
  "Load_smtp_password": "password"
}
```

### 2. Configurar Key Vault y permisos IAM

1. Crear un Azure Key Vault en tu suscripción.
2. Configurar los secretos:
   - **username**: Usuario del servidor SMTP.
   - **password**: Contraseña del servidor SMTP.
3. Asegúrate de asignar a la Azure Function acceso a Key Vault mediante roles de IAM adecuados (por ejemplo, "Key Vault Secrets User").

### 3. Dependencias

Las siguientes dependencias están configuradas en el proyecto:

- **Azure.Extensions.AspNetCore.Configuration.Secrets**: Para la integración con Key Vault.
- **System.Net.Mail**: Para el manejo de correos electrónicos.

## Ejecución de Pruebas

El proyecto incluye pruebas automatizadas configuradas con xUnit. Una prueba destacada es el envío de un correo electrónico mediante el servicio `IEmailService`.

### Ejecución de pruebas localmente

1. Configurar un archivo `local.settings.json` dentro del proyecto de pruebas (`PruebaAzureFunctionsTest`) con los mismos valores mencionados anteriormente.
2. Ejecutar las pruebas mediante el comando:

   ```bash
   dotnet test
   ```

## Notas Adicionales

- **Clase Result**: El proyecto incluye una clase genérica `Result` para gestionar respuestas de los servicios y proveedores:

  ```csharp
  public class Result
  {
      public bool isSuccess { get; set; }
      public string message { get; set; }
      public object data { get; set; }  
  }
  ```

- **Pipeline de CI/CD**: El pipeline en Azure DevOps está configurado para Compilar y desplegar automáticamente en Azure Functions.

## Licencia

Este proyecto no incluye una licencia específica.
