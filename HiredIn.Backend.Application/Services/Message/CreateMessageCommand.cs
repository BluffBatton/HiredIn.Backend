using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.MessagesDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Message
{
    public class CreateMessageCommand : IRequest<MessageReadDTO>
    {
        public MessageCreateDTO Message { get; set; }

        public CreateMessageCommand(MessageCreateDTO message)
        {
            Message = message;
        }
    }

    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly IChatRealtimeService _chatRealtimeService;
        private readonly INotificationService _notificationService;

        public CreateMessageCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IMapper mapper,
            IChatRealtimeService chatRealtimeService,
            INotificationService notificationService)
        {
            _context = context;
            _userContextService = userContextService;
            _mapper = mapper;
            _chatRealtimeService = chatRealtimeService;
            _notificationService = notificationService;
        }

        public async Task<MessageReadDTO> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var content = request.Message.Content?.Trim();

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("Message content cannot be empty.");

            var chat = await _context.Chats
                .FirstOrDefaultAsync(c =>
                    c.Id == request.Message.ChatId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (chat == null)
                throw new KeyNotFoundException("Chat not found.");

            var isParticipant = await _context.ChatParticipants
                .AnyAsync(cp =>
                    cp.ChatId == request.Message.ChatId &&
                    cp.UserId == currentUserId.Value,
                    cancellationToken);

            if (!isParticipant)
                throw new UnauthorizedAccessException("You do not have access to this chat.");

            var message = new Domain.Entities.Message
            {
                ChatId = request.Message.ChatId,
                SenderUserId = currentUserId.Value,
                Text = content,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var receiverUserIds = await _context.ChatParticipants
                .Where(cp =>
                    cp.ChatId == request.Message.ChatId &&
                    cp.UserId != currentUserId.Value)
                .Select(cp => cp.UserId)
                .ToListAsync(cancellationToken);

            foreach (var receiverUserId in receiverUserIds)
            {
                await _notificationService.CreateAsync(
                    receiverUserId,
                    NotificationType.NewMessage,
                    "Нове повідомлення",
                    "Ви отримали нове повідомлення в чаті.",
                    cancellationToken);
            }

            var savedMessage = await _context.Messages
                .Include(m => m.SenderUser)
                .FirstAsync(m => m.Id == message.Id, cancellationToken);

            var dto = _mapper.Map<MessageReadDTO>(savedMessage);

            await _chatRealtimeService.SendMessageToChatAsync(dto.ChatId, dto, cancellationToken);

            return dto;
        }
    }
}