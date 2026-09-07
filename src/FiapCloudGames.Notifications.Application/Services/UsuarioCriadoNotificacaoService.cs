using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Interfaces;

namespace FiapCloudGames.Notifications.Application.Services
{
    public class UsuarioCriadoNotificacaoService(IEmailService emailService)
    {
        private readonly IEmailService _emailService = emailService;

        public Task NotificarAsync(UsuarioCriadoEvent evento)
        {
            var texto = $"Bem-vindo, {evento.Nome}! Seu usuário foi criado com sucesso.";
            return _emailService.EnviarAsync(texto, evento.Email);
        }
    }
}
