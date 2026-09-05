namespace FiapCloudGames.Notifications.Application.Events
{
    public class PagamentoProcessadoEvent
    {
        public Guid PedidoId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
