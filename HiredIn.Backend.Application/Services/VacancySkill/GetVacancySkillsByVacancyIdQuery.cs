using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.VacancySkill
{
    public class GetVacancySkillsByVacancyIdQuery : IRequest<List<VacancySkillReadDTO>>
    {
        public Guid VacancyId { get; set; }

        public GetVacancySkillsByVacancyIdQuery(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }

    public class GetVacancySkillsByVacancyIdQueryHandler : IRequestHandler<GetVacancySkillsByVacancyIdQuery, List<VacancySkillReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetVacancySkillsByVacancyIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<VacancySkillReadDTO>> Handle(GetVacancySkillsByVacancyIdQuery request, CancellationToken cancellationToken)
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

            var vacancySkills = await _context.VacancySkills
                .Where(vs => vs.VacancyId == request.VacancyId)
                .Include(vs => vs.Skill)
                .OrderBy(vs => vs.Skill.Name)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VacancySkillReadDTO>>(vacancySkills);
        }
    }
}