using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FiapCloudGames.Notifications.Functions.Functions
{
    public class UsuarioCriadoFunction(
        UsuarioCriadoNotificacaoService service,
        ILogger<UsuarioCriadoFunction> logger)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly UsuarioCriadoNotificacaoService _service = service;
        private readonly ILogger<UsuarioCriadoFunction> _logger = logger;

        [Function(nameof(UsuarioCriadoFunction))]
        public async Task Run(
            [RabbitMQTrigger("usuario-criado", ConnectionStringSetting = "RabbitMQ")] string message)
        {
            _logger.LogInformation("Mensagem recebida na fila usuario-criado.");

            var evento = JsonSerializer.Deserialize<UsuarioCriadoEvent>(message, JsonOptions)
                ?? throw new InvalidOperationException("Payload UsuarioCriado inválido.");

            await _service.NotificarAsync(evento);
        }
    }
}
