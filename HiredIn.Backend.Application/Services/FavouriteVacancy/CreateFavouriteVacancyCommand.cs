using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs;
using HiredIn.Backend.Domain.Entities;
using MediatR;

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
            if(userId is null)
            {
                throw new UnauthorizedAccessException($"User is not authenticated");
            }

            var favouriteVacancy = new Domain.Entities.FavouriteVacancy
            {
                UserId = userId.Value,
                VacancyId = request.Dto.VacancyId
            };

            await _context.FavouriteVacancies.AddAsync(favouriteVacancy, cancellationToken);
            await _context.SaveChangesAsync();

            return;
        }
    }
}
