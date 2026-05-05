using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ChatDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Chat
{
    public class GetMyChatsQuery : IRequest<List<ChatReadDTO>>
    {
    }

    public class GetMyChatsQueryHandler : IRequestHandler<GetMyChatsQuery, List<ChatReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyChatsQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<List<ChatReadDTO>> Handle(
            GetMyChatsQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var chats = await _context.Chats
                .AsNoTracking()
                .Where(c =>
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
                .OrderByDescending(c => c.LastMessageCreatedAtUtc ?? c.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return chats;
        }
    }
}