using HiredIn.Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class UnarchiveVacancyCommand : IRequest
    {
        public Guid Id { get; set; }
        public UnarchiveVacancyCommand(Guid id)
        {
            Id = id;
        }
    }

    public class UnarchiveVacancyCommandHandler : IRequestHandler<UnarchiveVacancyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        public UnarchiveVacancyCommandHandler(IApplicationDbContext applicationDbContext, IUserContextService userContextService)
        {
            _context = applicationDbContext;
            _userContextService = userContextService;
        }
        public async Task Handle(UnarchiveVacancyCommand request, CancellationToken cancellationToken)
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
            vacancy.Status = Domain.Enums.VacancyStatus.Published;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
