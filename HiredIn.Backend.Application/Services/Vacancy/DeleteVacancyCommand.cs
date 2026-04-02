using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class DeleteVacancyCommand : IRequest
    {
        public Guid VacancyId { get; set; }

        public DeleteVacancyCommand(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }

    public class DeleteVacancyCommandHandler : IRequestHandler<DeleteVacancyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteVacancyCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteVacancyCommand request, CancellationToken cancellationToken)
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
                throw new UnauthorizedAccessException("You do not have permission to delete vacancies.");

            //vacancy.DeletedAtUtc = DateTime.UtcNow;
            vacancy.Status = VacancyStatus.Archived;
            vacancy.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}