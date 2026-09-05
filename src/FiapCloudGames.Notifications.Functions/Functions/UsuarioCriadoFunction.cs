using System.Text.Json;
using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Notifications.Functions.Functions
{
    public class UsuarioCriadoFunction
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly UsuarioCriadoNotificacaoService _service;
        private readonly ILogger<UsuarioCriadoFunction> _logger;

        public UsuarioCriadoFunction(
            UsuarioCriadoNotificacaoService service,
            ILogger<UsuarioCriadoFunction> logger)
        {
            _service = service;
            _logger = logger;
        }

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
