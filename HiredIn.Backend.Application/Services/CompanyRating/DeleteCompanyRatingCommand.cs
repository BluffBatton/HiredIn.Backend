using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyRating
{
    public class DeleteCompanyRatingCommand : IRequest
    {
        public Guid CompanyRatingId { get; set; }

        public DeleteCompanyRatingCommand(Guid companyRatingId)
        {
            CompanyRatingId = companyRatingId;
        }
    }

    public class DeleteCompanyRatingCommandHandler : IRequestHandler<DeleteCompanyRatingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteCompanyRatingCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteCompanyRatingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var rating = await _context.CompanyRatings
                .FirstOrDefaultAsync(cr =>
                    cr.Id == request.CompanyRatingId &&
                    cr.DeletedAtUtc == null,
                    cancellationToken);

            if (rating == null)
                throw new KeyNotFoundException("Company rating not found.");

            if (rating.UserId != userId.Value)
                throw new UnauthorizedAccessException("You can delete only your own rating.");

            rating.DeletedAtUtc = DateTime.UtcNow;
            rating.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}