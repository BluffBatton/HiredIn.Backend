using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AdminDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class GetAdminStatisticsQuery : IRequest<AdminStatisticsDTO>
    {
    }

    public class GetAdminStatisticsQueryHandler : IRequestHandler<GetAdminStatisticsQuery, AdminStatisticsDTO>
    {
        private readonly IApplicationDbContext _context;

        public GetAdminStatisticsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStatisticsDTO> Handle(GetAdminStatisticsQuery request, CancellationToken cancellationToken)
        {
            var users = _context.Users.AsNoTracking();
            var companies = _context.Companies.AsNoTracking().Where(c => c.DeletedAtUtc == null);
            var vacancies = _context.Vacancies.AsNoTracking().Where(v => v.DeletedAtUtc == null);

            return new AdminStatisticsDTO
            {
                TotalUsers = await users.CountAsync(cancellationToken),
                ActiveUsers = await users.CountAsync(u => u.Status == UserStatus.Active, cancellationToken),
                BlockedUsers = await users.CountAsync(u => u.Status == UserStatus.Blocked, cancellationToken),
                DeletedUsers = await users.CountAsync(u => u.Status == UserStatus.Deleted, cancellationToken),

                TotalCompanies = await companies.CountAsync(cancellationToken),
                PendingCompanies = await companies.CountAsync(c => c.Status == CompanyStatus.Pending, cancellationToken),
                ActiveCompanies = await companies.CountAsync(c => c.Status == CompanyStatus.Active, cancellationToken),
                BlockedCompanies = await companies.CountAsync(c => c.Status == CompanyStatus.Blocked, cancellationToken),
                ArchivedCompanies = await companies.CountAsync(c => c.Status == CompanyStatus.Archived, cancellationToken),

                TotalVacancies = await vacancies.CountAsync(cancellationToken),
                DraftVacancies = await vacancies.CountAsync(v => v.Status == VacancyStatus.Draft, cancellationToken),
                PublishedVacancies = await vacancies.CountAsync(v => v.Status == VacancyStatus.Published, cancellationToken),
                ArchivedVacancies = await vacancies.CountAsync(v => v.Status == VacancyStatus.Archived, cancellationToken),
                ClosedVacancies = await vacancies.CountAsync(v => v.Status == VacancyStatus.Closed, cancellationToken),

                TotalApplications = await _context.Applications
                    .AsNoTracking()
                    .CountAsync(a => a.DeletedAtUtc == null, cancellationToken),
                TotalCompanyRatings = await _context.CompanyRatings
                    .AsNoTracking()
                    .CountAsync(cr => cr.DeletedAtUtc == null, cancellationToken)
            };
        }
    }
}
