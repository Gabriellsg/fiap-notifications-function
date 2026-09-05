using System.Text.Json.Serialization;

namespace FiapCloudGames.Notifications.Application.Events
{
    /// <summary>
    /// Evento publicado pela UsersAPI na fila <c>usuario-criado</c>.
    /// Aceita <c>Id</c> (contrato atual do publisher) e <c>UsuarioId</c> (legado).
    /// </summary>
    public class UsuarioCriadoEvent
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; init; }

        [JsonPropertyName("UsuarioId")]
        public Guid UsuarioId { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public Guid UsuarioResolvido => Id != Guid.Empty ? Id : UsuarioId;
    }
}
