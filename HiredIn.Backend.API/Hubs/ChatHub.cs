using HiredIn.Backend.API.Common.RealTime;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Application.Services.Message;
using HiredIn.Backend.Contracts.MessagesDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HiredIn.Backend.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public ChatHub(
            IMediator mediator,
            IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task JoinChat(Guid chatId)
        {
            var currentUserId = GetCurrentUserId();

            var isParticipant = await _context.ChatParticipants
                .AnyAsync(cp =>
                    cp.ChatId == chatId &&
                    cp.UserId == currentUserId);

            if (!isParticipant)
                throw new HubException("You do not have access to this chat.");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                ChatGroupNames.ForChat(chatId));
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                ChatGroupNames.ForChat(chatId));
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

        private Guid GetCurrentUserId()
        {
            var userIdValue = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
                throw new HubException("User is not authenticated.");

            return userId;
        }
    }
}