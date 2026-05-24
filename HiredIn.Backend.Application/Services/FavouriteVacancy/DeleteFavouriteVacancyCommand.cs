using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.FavouriteVacancy
{
    public class DeleteFavouriteVacancyCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteFavouriteVacancyCommand(Guid id)
        {
            Id = id;
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

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authenticated");

            var favouriteVacancy = await _context.FavouriteVacancies
                .FirstOrDefaultAsync(fa =>
                    fa.UserId == userId.Value &&
                    fa.DeletedAtUtc == null &&
                    (fa.Id == request.Id || fa.VacancyId == request.Id),
                    cancellationToken);

            if (favouriteVacancy is null)
                throw new KeyNotFoundException("Favourite vacancy wasn't found");

            var now = DateTime.UtcNow;
            favouriteVacancy.DeletedAtUtc = now;
            favouriteVacancy.UpdatedAtUtc = now;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
