using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.MessagesDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Message
{
    public class GetMessagesByChatIdQuery : IRequest<List<MessageReadDTO>>
    {
        public Guid ChatId { get; set; }

        public GetMessagesByChatIdQuery(Guid chatId)
        {
            ChatId = chatId;
        }
    }

    public class GetMessagesByChatIdQueryHandler : IRequestHandler<GetMessagesByChatIdQuery, List<MessageReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public GetMessagesByChatIdQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _context = context;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<List<MessageReadDTO>> Handle(GetMessagesByChatIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var isParticipant = await _context.ChatParticipants
                .AnyAsync(cp => cp.ChatId == request.ChatId && cp.UserId == currentUserId.Value, cancellationToken);

            if (!isParticipant)
                throw new UnauthorizedAccessException("You do not have access to this chat.");

            var messages = await _context.Messages
                .Where(m => m.ChatId == request.ChatId && m.DeletedAtUtc == null)
                .Include(m => m.SenderUser)
                .OrderBy(m => m.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<MessageReadDTO>>(messages);
        }
    }
}
