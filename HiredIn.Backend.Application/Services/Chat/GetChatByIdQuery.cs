using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ChatDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Chat
{
    public class GetChatByIdQuery : IRequest<ChatReadDTO>
    {
        public Guid ChatId { get; set; }

        public GetChatByIdQuery(Guid chatId)
        {
            ChatId = chatId;
        }
    }

    public class GetChatByIdQueryHandler : IRequestHandler<GetChatByIdQuery, ChatReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetChatByIdQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<ChatReadDTO> Handle(
            GetChatByIdQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var chat = await _context.Chats
                .AsNoTracking()
                .Where(c =>
                    c.Id == request.ChatId &&
                    c.DeletedAtUtc == null &&
                    c.Participants.Any(p => p.UserId == currentUserId.Value))
                .Select(c => new ChatReadDTO
                {
                    Id = c.Id,
                    ApplicationId = c.ApplicationId,
                    Status = (Contracts.DTOs.Enums.ChatStatus)c.Status,
                    VacancyTitle = c.Application.Vacancy.Title,
                    CreatedAtUtc = c.CreatedAtUtc,

                    Participants = c.Participants
                        .Select(p => new ChatParticipantReadDTO
                        {
                            UserId = p.UserId,
                            FullName = p.User.FirstName + " " + p.User.LastName,
                            AvatarUrl = p.User.AvatarUrl
                        })
                        .ToList(),

                    LastMessageText = c.Messages
                        .OrderByDescending(m => m.CreatedAtUtc)
                        .Select(m => m.Text)
                        .FirstOrDefault(),

                    LastMessageCreatedAtUtc = c.Messages
                        .OrderByDescending(m => m.CreatedAtUtc)
                        .Select(m => (DateTime?)m.CreatedAtUtc)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (chat == null)
                throw new KeyNotFoundException("Chat not found.");

            return chat;
        }
    }
}