using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.FavouriteVacancy
{
    public class DeleteFavouriteVacancyCommand : IRequest
    {
        public Guid VacancyId { get; set; }

        public DeleteFavouriteVacancyCommand(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }
    
    public class DeleteFavouriteVacancyCommandHandler : IRequestHandler<DeleteFavouriteVacancyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        public DeleteFavouriteVacancyCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteFavouriteVacancyCommand request, CancellationToken cancellationToken)
        {
            Guid? userId = _userContextService.GetCurrentUserId();

            if (userId is null) throw new UnauthorizedAccessException("User is not authenticated");

            var favouriteVacancy = await _context.FavouriteVacancies.FirstOrDefaultAsync(fa => fa.Id == userId, cancellationToken);
            
            if (favouriteVacancy is null) throw new KeyNotFoundException("Favourite vacancy wasn't found");

            favouriteVacancy.DeletedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}
