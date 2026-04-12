using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.FavouriteVacancy
{
    public class GetFavouriteVacancyByIdQuery : IRequest<FavouriteVacancyReadDTO>
    {
        public Guid Id { get; set; }

        public GetFavouriteVacancyByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetFavouriteVacancyByIdQueryHandler : IRequestHandler<GetFavouriteVacancyByIdQuery, FavouriteVacancyReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetFavouriteVacancyByIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<FavouriteVacancyReadDTO> Handle(GetFavouriteVacancyByIdQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var favouriteVacancyDto = await _context.FavouriteVacancies
                .Where(fv => fv.Id == request.Id && fv.UserId == userId)
                .Select(fv => new FavouriteVacancyReadDTO
                {
                    Id = fv.Id,
                    VacancyId = fv.VacancyId,
                    VacancyName = fv.Vacancy.Title ?? string.Empty,
                    CompanyName = fv.Vacancy.Company.Name ?? string.Empty,
                    City = fv.Vacancy.City ?? string.Empty,
                    Salary = fv.Vacancy.SalaryMin 
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (favouriteVacancyDto == null)
            {
                throw new KeyNotFoundException($"Favourite vacancy with id '{request.Id}' was not found.");
            }

            return favouriteVacancyDto;
        }
    }
}
