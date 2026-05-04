using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyRating
{
    public class GetCompanyRatingByIdQuery : IRequest<CompanyRatingReadDTO>
    {
        public Guid CompanyRatingId { get; set; }

        public GetCompanyRatingByIdQuery(Guid companyRatingId)
        {
            CompanyRatingId = companyRatingId;
        }
    }

    public class GetCompanyRatingByIdQueryHandler : IRequestHandler<GetCompanyRatingByIdQuery, CompanyRatingReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompanyRatingByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CompanyRatingReadDTO> Handle(
            GetCompanyRatingByIdQuery request,
            CancellationToken cancellationToken)
        {
            var rating = await _context.CompanyRatings
                .AsNoTracking()
                .Include(cr => cr.Company)
                .Include(cr => cr.User)
                .FirstOrDefaultAsync(cr =>
                    cr.Id == request.CompanyRatingId &&
                    cr.DeletedAtUtc == null,
                    cancellationToken);

            if (rating == null)
                throw new KeyNotFoundException("Company rating not found.");

            return _mapper.Map<CompanyRatingReadDTO>(rating);
        }
    }
}