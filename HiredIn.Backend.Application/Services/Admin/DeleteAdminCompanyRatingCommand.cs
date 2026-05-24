using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class DeleteAdminCompanyRatingCommand : IRequest
    {
        public Guid RatingId { get; set; }

        public DeleteAdminCompanyRatingCommand(Guid ratingId)
        {
            RatingId = ratingId;
        }
    }

    public class DeleteAdminCompanyRatingCommandHandler : IRequestHandler<DeleteAdminCompanyRatingCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAdminCompanyRatingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAdminCompanyRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = await _context.CompanyRatings
                .FirstOrDefaultAsync(cr =>
                    cr.Id == request.RatingId &&
                    cr.DeletedAtUtc == null,
                    cancellationToken);

            if (rating == null)
                throw new KeyNotFoundException("Company rating not found.");

            rating.DeletedAtUtc = DateTime.UtcNow;
            rating.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
