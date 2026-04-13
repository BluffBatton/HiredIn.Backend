using HiredIn.Backend.API.Common.RealTime;
using HiredIn.Backend.API.Hubs;
using HiredIn.Backend.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HiredIn.Backend.API.Services
{
    public class ChatRealtimeService : IChatRealtimeService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRealtimeService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToChatAsync(Guid chatId, object payload, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(ChatGroupNames.ForChat(chatId))
                .SendAsync("ReceiveMessage", payload, cancellationToken);
        }
    }
}
