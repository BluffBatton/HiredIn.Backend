using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.NotificationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Notification
{
    public class GetMyNotificationsQuery : IRequest<List<NotificationReadDTO>>
    {
    }

    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetMyNotificationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<NotificationReadDTO>> Handle(
            GetMyNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n =>
                    n.UserId == userId.Value &&
                    n.DeletedAtUtc == null)
                .OrderBy(n => n.IsRead)
                .ThenByDescending(n => n.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<NotificationReadDTO>>(notifications);
        }
    }
}