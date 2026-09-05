using System.Text.Json;
using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Notifications.Functions.Functions
{
    public class PagamentoProcessadoFunction
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly PagamentoProcessadoNotificacaoService _service;
        private readonly ILogger<PagamentoProcessadoFunction> _logger;

        public PagamentoProcessadoFunction(
            PagamentoProcessadoNotificacaoService service,
            ILogger<PagamentoProcessadoFunction> logger)
        {
            _service = service;
            _logger = logger;
        }

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
