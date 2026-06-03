using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.Enums;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Application
{
    public class UpdateApplicationStatusCommand : IRequest<ApplicationReadDTO>
    {
        public Guid ApplicationId { get; set; }
        public ApplicationStatusUpdateDTO Status { get; set; }

        public UpdateApplicationStatusCommand(Guid applicationId, ApplicationStatusUpdateDTO status)
        {
            ApplicationId = applicationId;
            Status = status;
        }
    }

    public class UpdateApplicationStatusCommandHandler : IRequestHandler<UpdateApplicationStatusCommand, ApplicationReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;

        public UpdateApplicationStatusCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService,
            INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
            _notificationService = notificationService;
        }

        public async Task<ApplicationReadDTO> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var application = await _context.Applications
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.Company)
                .Include(a => a.Resume)
                    .ThenInclude(r => r.CandidateProfile)
                        .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(a =>
                    a.Id == request.ApplicationId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == application.Vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role is not (Domain.Enums.CompanyMemberRole)Contracts.DTOs.Enums.CompanyMemberRole.Owner and
                not (Domain.Enums.CompanyMemberRole)Contracts.DTOs.Enums.CompanyMemberRole.Recruiter)
                throw new UnauthorizedAccessException("You do not have permission to update application status.");

            var newStatus = (Domain.Enums.ApplicationStatus)request.Status.Status;

            if (application.Status == Domain.Enums.ApplicationStatus.Accepted)
                throw new InvalidOperationException("Accepted application cannot be changed.");

            if (application.Status == Domain.Enums.ApplicationStatus.Rejected)
                throw new InvalidOperationException("Rejected application cannot be changed.");

            if (newStatus == Domain.Enums.ApplicationStatus.Submitted)
                throw new InvalidOperationException("Cannot change application status back to Submitted.");

            application.Status = newStatus;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.CreateAsync(
                application.Resume.CandidateProfile.UserId,
                (Domain.Enums.NotificationType)Contracts.DTOs.Enums.NotificationType.ApplicationStatusChanged,
                "Статус заявки змінено",
                $"Статус вашої заявки на вакансію \"{application.Vacancy.Title}\" змінено на \"{application.Status}\".",
                cancellationToken);

            return _mapper.Map<ApplicationReadDTO>(application);
        }
    }
}
