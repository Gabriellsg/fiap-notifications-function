using FiapCloudGames.Notifications.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Notifications.Infrastructure.Email
{
    /// <summary>
    /// Simula envio de e-mail (mesmo comportamento da NotificationsAPI).
    /// </summary>
    public class SimuladorEmailService : IEmailService
    {
        private readonly ILogger<SimuladorEmailService> _logger;

        public SimuladorEmailService(ILogger<SimuladorEmailService> logger)
        {
            _logger = logger;
        }

        public Task EnviarAsync(string texto, string email)
        {
            _logger.LogInformation("Enviando e-mail para {Email}: {Texto}", email, texto);
            return Task.CompletedTask;
        }
    }
}
