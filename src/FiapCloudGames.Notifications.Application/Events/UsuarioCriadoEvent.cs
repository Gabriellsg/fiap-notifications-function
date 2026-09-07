using System.Text.Json.Serialization;

namespace FiapCloudGames.Notifications.Application.Events
{
    /// <summary>
    /// Evento publicado pela UsersAPI na fila <c>usuario-criado</c>.
    /// Aceita <c>Id</c> (contrato atual do publisher) e <c>UsuarioId</c> (legado).
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="UsuarioId"></param>
    public record UsuarioCriadoEvent([property: JsonPropertyName("Id")] Guid Id, [property: JsonPropertyName("UsuarioId")] Guid UsuarioId)
    {
        public string Nome { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public Guid UsuarioResolvido => Id != Guid.Empty ? Id : UsuarioId;
    }
}
