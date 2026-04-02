using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.VacancySkill
{
    public class PatchVacancySkillCommand : IRequest
    {
        public Guid VacancyId { get; set; }
        public Guid SkillId { get; set; }
        public VacancySkillPatchDTO VacancySkill { get; set; }

        public PatchVacancySkillCommand(Guid vacancyId, Guid skillId, VacancySkillPatchDTO vacancySkill)
        {
            VacancyId = vacancyId;
            SkillId = skillId;
            VacancySkill = vacancySkill;
        }
    }

    public class PatchVacancySkillCommandHandler : IRequestHandler<PatchVacancySkillCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public PatchVacancySkillCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchVacancySkillCommand request, CancellationToken cancellationToken)
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

            var vacancySkill = await _context.VacancySkills
                .FirstOrDefaultAsync(vs =>
                    vs.VacancyId == request.VacancyId &&
                    vs.SkillId == request.SkillId,
                    cancellationToken);

            if (vacancySkill == null)
                throw new KeyNotFoundException("Vacancy skill not found.");

            if (request.VacancySkill.IsRequired.HasValue)
                vacancySkill.IsRequired = request.VacancySkill.IsRequired.Value;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}