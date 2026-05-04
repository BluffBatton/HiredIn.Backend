using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyRating
{
    public class GetCompanyRatingsByCompanyIdQuery : IRequest<List<CompanyRatingReadDTO>>
    {
        public Guid CompanyId { get; set; }

        public GetCompanyRatingsByCompanyIdQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }

    public class GetCompanyRatingsByCompanyIdQueryHandler : IRequestHandler<GetCompanyRatingsByCompanyIdQuery, List<CompanyRatingReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompanyRatingsByCompanyIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyRatingReadDTO>> Handle(
            GetCompanyRatingsByCompanyIdQuery request,
            CancellationToken cancellationToken)
        {
            var companyExists = await _context.Companies
                .AnyAsync(c =>
                    c.Id == request.CompanyId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (!companyExists)
                throw new KeyNotFoundException("Company not found.");

            var ratings = await _context.CompanyRatings
                .AsNoTracking()
                .Include(cr => cr.Company)
                .Include(cr => cr.User)
                .Where(cr =>
                    cr.CompanyId == request.CompanyId &&
                    cr.DeletedAtUtc == null)
                .OrderByDescending(cr => cr.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CompanyRatingReadDTO>>(ratings);
        }
    }
}