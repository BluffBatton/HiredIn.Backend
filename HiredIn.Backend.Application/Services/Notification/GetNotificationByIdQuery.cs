using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.NotificationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Notification
{
    public class GetNotificationByIdQuery : IRequest<NotificationReadDTO>
    {
        public Guid NotificationId { get; set; }

        public GetNotificationByIdQuery(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }

    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetNotificationByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<NotificationReadDTO> Handle(
            GetNotificationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var notification = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n =>
                    n.Id == request.NotificationId &&
                    n.UserId == userId.Value &&
                    n.DeletedAtUtc == null,
                    cancellationToken);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            return _mapper.Map<NotificationReadDTO>(notification);
        }
    }
}