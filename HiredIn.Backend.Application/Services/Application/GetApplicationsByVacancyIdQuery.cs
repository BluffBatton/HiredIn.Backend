using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace HiredIn.Backend.Application.Services.Application
{
    public class GetApplicationsByVacancyIdQuery : IRequest<List<ApplicationReadDTO>>
    {
        public Guid VacancyId { get; set; }

        public GetApplicationsByVacancyIdQuery(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }

    public class GetApplicationsByVacancyIdQueryHandler : IRequestHandler<GetApplicationsByVacancyIdQuery, List<ApplicationReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetApplicationsByVacancyIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ApplicationReadDTO>> Handle(GetApplicationsByVacancyIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            var hasAccess = await _context.CompanyMembers
                .AnyAsync(cm =>
                    cm.CompanyId == vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (!hasAccess)
                throw new UnauthorizedAccessException("You do not have access to this vacancy.");

            var applications = await _context.Applications
                .Where(a => a.VacancyId == request.VacancyId && a.DeletedAtUtc == null)
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.Company)
                .Include(a => a.Resume)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ApplicationReadDTO>>(applications);
        }
    }
}
