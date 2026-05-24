using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class DeleteAdminCompanyCommand : IRequest
    {
        public Guid CompanyId { get; set; }

        public DeleteAdminCompanyCommand(Guid companyId)
        {
            CompanyId = companyId;
        }
    }

    public class DeleteAdminCompanyCommandHandler : IRequestHandler<DeleteAdminCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAdminCompanyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAdminCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == request.CompanyId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            var now = DateTime.UtcNow;

            company.Status = CompanyStatus.Archived;
            company.DeletedAtUtc = now;
            company.UpdatedAtUtc = now;

            var vacancies = await _context.Vacancies
                .Where(v =>
                    v.CompanyId == request.CompanyId &&
                    v.DeletedAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var vacancy in vacancies)
            {
                vacancy.Status = VacancyStatus.Archived;
                vacancy.DeletedAtUtc = now;
                vacancy.UpdatedAtUtc = now;
            }

            var ratings = await _context.CompanyRatings
                .Where(cr =>
                    cr.CompanyId == request.CompanyId &&
                    cr.DeletedAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var rating in ratings)
            {
                rating.DeletedAtUtc = now;
                rating.UpdatedAtUtc = now;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
