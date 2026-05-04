using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyRating
{
    public class CreateCompanyRatingCommand : IRequest<Guid>
    {
        public CompanyRatingCreateDTO Dto { get; set; }

        public CreateCompanyRatingCommand(CompanyRatingCreateDTO dto)
        {
            Dto = dto;
        }
    }

    public class CreateCompanyRatingCommandHandler : IRequestHandler<CreateCompanyRatingCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateCompanyRatingCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateCompanyRatingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (request.Dto.Rating < 1 || request.Dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.Dto.CompanyId && c.DeletedAtUtc == null, cancellationToken);

            if (!companyExists)
                throw new KeyNotFoundException("Company not found.");

            var alreadyRated = await _context.CompanyRatings
                .AnyAsync(cr =>
                    cr.CompanyId == request.Dto.CompanyId &&
                    cr.UserId == userId.Value &&
                    cr.DeletedAtUtc == null,
                    cancellationToken);

            if (alreadyRated)
                throw new InvalidOperationException("You have already rated this company.");

            var companyRating = _mapper.Map<Domain.Entities.CompanyRating>(request.Dto);

            companyRating.UserId = userId.Value;
            companyRating.CreatedAtUtc = DateTime.UtcNow;
            companyRating.UpdatedAtUtc = DateTime.UtcNow;

            await _context.CompanyRatings.AddAsync(companyRating, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return companyRating.Id;
        }
    }
}