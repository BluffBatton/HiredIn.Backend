using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class GetAdminCompanyRatingsQuery : IRequest<List<CompanyRatingReadDTO>>
    {
    }

    public class GetAdminCompanyRatingsQueryHandler : IRequestHandler<GetAdminCompanyRatingsQuery, List<CompanyRatingReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAdminCompanyRatingsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyRatingReadDTO>> Handle(
            GetAdminCompanyRatingsQuery request,
            CancellationToken cancellationToken)
        {
            var ratings = await _context.CompanyRatings
                .AsNoTracking()
                .Include(cr => cr.Company)
                .Include(cr => cr.User)
                .Where(cr => cr.DeletedAtUtc == null)
                .OrderByDescending(cr => cr.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CompanyRatingReadDTO>>(ratings);
        }
    }
}
