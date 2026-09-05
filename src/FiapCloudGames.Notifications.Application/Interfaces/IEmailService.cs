namespace FiapCloudGames.Notifications.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarAsync(string texto, string email);
    }
}
