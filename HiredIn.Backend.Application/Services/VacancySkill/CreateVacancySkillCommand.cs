using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainVacancySkill = HiredIn.Backend.Domain.Entities.VacancySkill;

namespace HiredIn.Backend.Application.Services.VacancySkill
{
    public class CreateVacancySkillCommand : IRequest<VacancySkillReadDTO>
    {
        public VacancySkillCreateDTO VacancySkill { get; set; }

        public CreateVacancySkillCommand(VacancySkillCreateDTO vacancySkill)
        {
            VacancySkill = vacancySkill;
        }
    }

    public class CreateVacancySkillCommandHandler : IRequestHandler<CreateVacancySkillCommand, VacancySkillReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateVacancySkillCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<VacancySkillReadDTO> Handle(CreateVacancySkillCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var vacancy = await _context.Vacancies
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancySkill.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role != CompanyMemberRole.Owner &&
                membership.Role != CompanyMemberRole.Recruiter)
                throw new UnauthorizedAccessException("You do not have permission to manage vacancy skills.");

            var skill = await _context.Skills
                .FirstOrDefaultAsync(s =>
                    s.Id == request.VacancySkill.SkillId &&
                    s.DeletedAtUtc == null,
                    cancellationToken);

            if (skill == null)
                throw new KeyNotFoundException("Skill not found.");

            var alreadyExists = await _context.VacancySkills
                .AnyAsync(vs =>
                    vs.VacancyId == request.VacancySkill.VacancyId &&
                    vs.SkillId == request.VacancySkill.SkillId,
                    cancellationToken);

            if (alreadyExists)
                throw new InvalidOperationException("This skill is already added to the vacancy.");

            var vacancySkill = _mapper.Map<DomainVacancySkill>(request.VacancySkill);
            vacancySkill.Vacancy = vacancy;
            vacancySkill.Skill = skill;

            await _context.VacancySkills.AddAsync(vacancySkill, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<VacancySkillReadDTO>(vacancySkill);
        }
    }
}