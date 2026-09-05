using FiapCloudGames.Notifications.Application.Events;
using FiapCloudGames.Notifications.Application.Interfaces;

namespace FiapCloudGames.Notifications.Application.Services
{
    public class PagamentoProcessadoNotificacaoService
    {
        private readonly IEmailService _emailService;

        public PagamentoProcessadoNotificacaoService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task NotificarAsync(PagamentoProcessadoEvent evento)
        {
            var texto = evento.Status == "Aprovado"
                ? $"Olá, {evento.NomeUsuario}! Seu pedido {evento.PedidoId} foi processado com sucesso."
                : $"Olá, {evento.NomeUsuario}! Infelizmente, seu pedido {evento.PedidoId} não foi aprovado. Por favor, entre em contato com o suporte para mais informações.";

            return _emailService.EnviarAsync(texto, evento.Email);
        }
    }
}
