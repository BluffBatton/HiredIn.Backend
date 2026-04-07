using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class ArchiveVacancyCommand : IRequest
    {
        public Guid Id { get; set; }
        public ArchiveVacancyCommand(Guid id)
        {
            Id = id;
        }
    }

    public class ArchiveVacancyCommandHandler : IRequestHandler<ArchiveVacancyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public ArchiveVacancyCommandHandler( IApplicationDbContext applicationDbContext, IUserContextService userContextService)
        {
            _context = applicationDbContext;
            _userContextService = userContextService;
        }

        public async Task Handle(ArchiveVacancyCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            var ifCompanyMemberTrue = await _context.CompanyMembers
                .AnyAsync(x => x.UserId == userId && x.Company != null && x.Company.Vacancies.Any(v => v.Id == request.Id),
                              cancellationToken);

            if (!ifCompanyMemberTrue)
            {
                throw new UnauthorizedAccessException("User is not a member of the company that owns this vacancy.");
            }

            var vacancy = await _context.Vacancies.FindAsync(new object[] { request.Id }, cancellationToken);

            if (vacancy == null)
            {
                throw new KeyNotFoundException($"Vacancy with id '{request.Id}' was not found.");
            }

            vacancy.Status = Domain.Enums.VacancyStatus.Archived;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
