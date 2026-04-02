using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.VacancySkill
{
    public class DeleteVacancySkillCommand : IRequest
    {
        public Guid VacancyId { get; set; }
        public Guid SkillId { get; set; }

        public DeleteVacancySkillCommand(Guid vacancyId, Guid skillId)
        {
            VacancyId = vacancyId;
            SkillId = skillId;
        }
    }

    public class DeleteVacancySkillCommandHandler : IRequestHandler<DeleteVacancySkillCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteVacancySkillCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteVacancySkillCommand request, CancellationToken cancellationToken)
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

            _context.VacancySkills.Remove(vacancySkill);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}