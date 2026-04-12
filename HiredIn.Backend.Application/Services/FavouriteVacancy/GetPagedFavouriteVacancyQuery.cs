using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.FavouriteVacancy
{
    public class GetPagedFavouriteVacancyQuery : IRequest<PaginatedList<FavouriteVacancyReadDTO>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

    }

    public class GetPagedFavouriteVacancyQueryHandler : IRequestHandler<GetPagedFavouriteVacancyQuery, PaginatedList<FavouriteVacancyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetPagedFavouriteVacancyQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<PaginatedList<FavouriteVacancyReadDTO>> Handle(
            GetPagedFavouriteVacancyQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var pageIndex = request.Page <= 0 ? 0 : request.Page - 1;
            var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

            var favouriteVacanciesQuery = _context.FavouriteVacancies
                .AsNoTracking()
                .Where(fv =>
                    fv.UserId == userId.Value &&
                    fv.DeletedAtUtc == null &&
                    fv.Vacancy.DeletedAtUtc == null)
                .OrderByDescending(fv => fv.CreatedAtUtc)
                .Select(fv => new FavouriteVacancyReadDTO
                {
                    Id = fv.Id,
                    VacancyId = fv.VacancyId,
                    VacancyName = fv.Vacancy.Title,
                    CompanyName = fv.Vacancy.Company.Name,
                    City = fv.Vacancy.City,
                    Salary = fv.Vacancy.SalaryMin
                });

            return await PaginatedList<FavouriteVacancyReadDTO>.CreateAsync(
                favouriteVacanciesQuery,
                pageIndex,
                pageSize,
                cancellationToken);
        }
    }
}