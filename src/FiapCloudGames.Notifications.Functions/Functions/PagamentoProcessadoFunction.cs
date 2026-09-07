using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FiapCloudGames.Notifications.Functions.Functions
{
    public class PagamentoProcessadoFunction(
        PagamentoProcessadoNotificacaoService service,
        ILogger<PagamentoProcessadoFunction> logger)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly PagamentoProcessadoNotificacaoService _service = service;
        private readonly ILogger<PagamentoProcessadoFunction> _logger = logger;

        [Function(nameof(PagamentoProcessadoFunction))]
        public async Task Run(
            [RabbitMQTrigger("pagamento-processado-notifications", ConnectionStringSetting = "RabbitMQ")] string message)
        {
            _logger.LogInformation("Mensagem recebida na fila pagamento-processado-notifications.");

            var evento = JsonSerializer.Deserialize<PagamentoProcessadoEvent>(message, JsonOptions)
                ?? throw new InvalidOperationException("Payload PagamentoProcessado inválido.");

            await _service.NotificarAsync(evento);
        }
    }
}
