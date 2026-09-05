using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Interfaces;

namespace FiapCloudGames.Notifications.Application.Services
{
    public class UsuarioCriadoNotificacaoService
    {
        private readonly IEmailService _emailService;

        public UsuarioCriadoNotificacaoService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task NotificarAsync(UsuarioCriadoEvent evento)
        {
            var texto = $"Bem-vindo, {evento.Nome}! Seu usuário foi criado com sucesso.";
            return _emailService.EnviarAsync(texto, evento.Email);
        }
    }
}
