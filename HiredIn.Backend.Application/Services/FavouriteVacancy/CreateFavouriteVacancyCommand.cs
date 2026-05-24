using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.FavouriteVacancy
{
    public class CreateFavouriteVacancyCommand : IRequest
    {
        public FavouriteVacancyCreateDTO Dto { get; set; }

        public CreateFavouriteVacancyCommand(FavouriteVacancyCreateDTO dto)
        {
            Dto = dto;
        }
    }

    public class CreateFavouriteVacancyCommandHandler : IRequestHandler<CreateFavouriteVacancyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateFavouriteVacancyCommandHandler(IUserContextService userContextService, IApplicationDbContext context)
        {
            _userContextService = userContextService;
            _context = context;
        }

        public async Task Handle(CreateFavouriteVacancyCommand request, CancellationToken cancellationToken)
        {
            Guid? userId = _userContextService.GetCurrentUserId();
            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var vacancyExists = await _context.Vacancies
                .AnyAsync(v =>
                    v.Id == request.Dto.VacancyId &&
                    v.DeletedAtUtc == null &&
                    v.Status == VacancyStatus.Published &&
                    v.Company.DeletedAtUtc == null &&
                    v.Company.Status == CompanyStatus.Active,
                    cancellationToken);

            if (!vacancyExists)
                throw new KeyNotFoundException("Vacancy not found.");

            var now = DateTime.UtcNow;
            var existingFavourite = await _context.FavouriteVacancies
                .FirstOrDefaultAsync(fv =>
                    fv.UserId == userId.Value &&
                    fv.VacancyId == request.Dto.VacancyId,
                    cancellationToken);

            if (existingFavourite != null)
            {
                if (existingFavourite.Id == Guid.Empty)
                    existingFavourite.Id = Guid.NewGuid();

                existingFavourite.DeletedAtUtc = null;
                existingFavourite.UpdatedAtUtc = now;

                if (existingFavourite.CreatedAtUtc == default)
                    existingFavourite.CreatedAtUtc = now;

                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            var favouriteVacancy = new Domain.Entities.FavouriteVacancy
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                VacancyId = request.Dto.VacancyId,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _context.FavouriteVacancies.AddAsync(favouriteVacancy, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
