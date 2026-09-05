using FiapCloudGames.Notifications.Application.Interfaces;
using FiapCloudGames.Notifications.Application.Services;
using FiapCloudGames.Notifications.Infrastructure.Email;
using FiapCloudGames.Notifications.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Notifications.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IEmailService, SimuladorEmailService>();
            services.AddSingleton<UsuarioCriadoNotificacaoService>();
            services.AddSingleton<PagamentoProcessadoNotificacaoService>();
            services.AddHostedService<RabbitMqTopologyInitializer>();
            return services;
        }
    }
}
