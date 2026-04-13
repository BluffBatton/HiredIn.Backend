using HiredIn.Backend.API.Common.RealTime;
using HiredIn.Backend.Application.Services.Message;
using HiredIn.Backend.Contracts.MessagesDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HiredIn.Backend.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroupNames.ForChat(chatId));
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroupNames.ForChat(chatId));
        }

        public async Task<MessageReadDTO> SendMessage(Guid chatId, string content)
        {
            return await _mediator.Send(
                new CreateMessageCommand(new MessageCreateDTO
                {
                    ChatId = chatId,
                    Content = content
                }));
        }
    }
}
