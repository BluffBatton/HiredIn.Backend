using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ChatDTOs;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Chat
{
    public class CreateChatCommand : IRequest<Guid>
    {
        public ChatCreateDTO Chat { get; set; }

        public CreateChatCommand(ChatCreateDTO chat)
        {
            Chat = chat;
        }
    }

    public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateChatCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var application = await _context.Applications
                .Include(a => a.Resume)
                    .ThenInclude(r => r.CandidateProfile)
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a =>
                    a.Id == request.Chat.ApplicationId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            var candidateUserId = application.Resume.CandidateProfile.UserId;

            var isCandidate = candidateUserId == currentUserId.Value;

            var isCompanyMember = await _context.CompanyMembers
                .AnyAsync(cm =>
                    cm.CompanyId == application.Vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (!isCandidate && !isCompanyMember)
                throw new UnauthorizedAccessException("You do not have access to this application.");

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(c =>
                    c.ApplicationId == request.Chat.ApplicationId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (existingChat != null)
                return existingChat.Id;

            var companyMemberUserIds = await _context.CompanyMembers
                .Where(cm => cm.CompanyId == application.Vacancy.CompanyId)
                .Select(cm => cm.UserId)
                .ToListAsync(cancellationToken);

            if (!companyMemberUserIds.Any())
                throw new InvalidOperationException("Company has no members.");

            var participantUserIds = companyMemberUserIds
                .Append(candidateUserId)
                .Distinct()
                .ToList();

            var chat = new Domain.Entities.Chat
            {
                ApplicationId = application.Id,
                Status = ChatStatus.Active,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.Chats.AddAsync(chat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var participants = participantUserIds.Select(userId => new ChatParticipant
            {
                ChatId = chat.Id,
                UserId = userId
            });

            await _context.ChatParticipants.AddRangeAsync(participants, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return chat.Id;
        }
    }
}