using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Application
{
    public class CreateApplicationCommand : IRequest<ApplicationReadDTO>
    {
        public ApplicationCreateDTO Application { get; set; }

        public CreateApplicationCommand(ApplicationCreateDTO application)
        {
            Application = application;
        }
    }

    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, ApplicationReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;

        public CreateApplicationCommandHandler(
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

        public async Task<ApplicationReadDTO> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resume = await _context.Resumes
                .Include(r => r.CandidateProfile)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.Application.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You can apply only with your own resume.");

            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v =>
                    v.Id == request.Application.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            if (vacancy.Status != VacancyStatus.Published)
                throw new InvalidOperationException("You cannot apply to inactive vacancy.");

            var alreadyApplied = await _context.Applications
                .AnyAsync(a =>
                    a.VacancyId == request.Application.VacancyId &&
                    a.ResumeId == request.Application.ResumeId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (alreadyApplied)
                throw new InvalidOperationException("You have already applied to this vacancy.");

            var application = _mapper.Map<Domain.Entities.Application>(request.Application);

            application.Status = ApplicationStatus.Submitted;
            application.CreatedAtUtc = DateTime.UtcNow;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Applications.AddAsync(application, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var companyMemberUserIds = await _context.CompanyMembers
                .Where(cm => cm.CompanyId == vacancy.CompanyId)
                .Select(cm => cm.UserId)
                .ToListAsync(cancellationToken);

            foreach (var employerUserId in companyMemberUserIds)
            {
                await _notificationService.CreateAsync(
                    employerUserId,
                    NotificationType.NewApplication,
                    "Нова заявка на вакансію",
                    $"Кандидат подав заявку на вакансію \"{vacancy.Title}\".",
                    cancellationToken);
            }

            var createdApplication = await _context.Applications
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.Company)
                .Include(a => a.Resume)
                    .ThenInclude(r => r.CandidateProfile)
                        .ThenInclude(cp => cp.User)
                .FirstAsync(a => a.Id == application.Id, cancellationToken);

            return _mapper.Map<ApplicationReadDTO>(createdApplication);
        }
    }
}
