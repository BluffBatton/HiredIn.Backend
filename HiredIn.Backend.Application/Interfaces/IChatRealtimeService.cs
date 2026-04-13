namespace HiredIn.Backend.Application.Interfaces
{
    public interface IChatRealtimeService
    {
        Task SendMessageToChatAsync(Guid chatId, object payload, CancellationToken cancellationToken = default);
    }
}
