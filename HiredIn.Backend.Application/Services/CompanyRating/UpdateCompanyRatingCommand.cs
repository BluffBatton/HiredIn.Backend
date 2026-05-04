using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyRating
{
    public class UpdateCompanyRatingCommand : IRequest
    {
        public Guid CompanyRatingId { get; set; }
        public CompanyRatingUpdateDTO Dto { get; set; }

        public UpdateCompanyRatingCommand(Guid companyRatingId, CompanyRatingUpdateDTO dto)
        {
            CompanyRatingId = companyRatingId;
            Dto = dto;
        }
    }

    public class UpdateCompanyRatingCommandHandler : IRequestHandler<UpdateCompanyRatingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public UpdateCompanyRatingCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(UpdateCompanyRatingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (request.Dto.Rating < 1 || request.Dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var rating = await _context.CompanyRatings
                .FirstOrDefaultAsync(cr =>
                    cr.Id == request.CompanyRatingId &&
                    cr.DeletedAtUtc == null,
                    cancellationToken);

            if (rating == null)
                throw new KeyNotFoundException("Company rating not found.");

            if (rating.UserId != userId.Value)
                throw new UnauthorizedAccessException("You can update only your own rating.");

            _mapper.Map(request.Dto, rating);

            rating.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}