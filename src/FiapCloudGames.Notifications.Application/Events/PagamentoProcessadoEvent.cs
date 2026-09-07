namespace FiapCloudGames.Notifications.Application.Events
{
    public record PagamentoProcessadoEvent(Guid PedidoId, decimal Valor)
    {
        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
