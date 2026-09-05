using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FiapCloudGames.Notifications.Infrastructure.Messaging
{
    /// <summary>
    /// Garante filas/exchanges usados pelos triggers (mesmo topologia da NotificationsAPI).
    /// </summary>
    public class RabbitMqTopologyInitializer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqTopologyInitializer> _logger;

        public RabbitMqTopologyInitializer(
            IConfiguration configuration,
            ILogger<RabbitMqTopologyInitializer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = CreateFactory();
                await using var connection = await factory.CreateConnectionAsync(cancellationToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await channel.QueueDeclareAsync(
                    queue: "usuario-criado",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.ExchangeDeclareAsync(
                    exchange: "pagamento-processado",
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueDeclareAsync(
                    queue: "pagamento-processado-notifications",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: "pagamento-processado-notifications",
                    exchange: "pagamento-processado",
                    routingKey: string.Empty,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Topologia RabbitMQ de notificações pronta.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Não foi possível declarar a topologia RabbitMQ no startup. " +
                    "As Functions ainda podem consumir se as filas já existirem.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private ConnectionFactory CreateFactory()
        {
            var amqp = _configuration["RabbitMQ"]
                ?? _configuration.GetConnectionString("RabbitMQ");

            if (!string.IsNullOrWhiteSpace(amqp))
            {
                return new ConnectionFactory { Uri = new Uri(amqp) };
            }

            return new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMq:Username"] ?? "admin",
                Password = _configuration["RabbitMq:Password"] ?? "rabbitmq123"
            };
        }
    }
}
