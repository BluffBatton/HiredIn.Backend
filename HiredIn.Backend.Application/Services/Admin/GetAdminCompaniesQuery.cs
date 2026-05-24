using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class GetAdminCompaniesQuery : IRequest<List<CompanyReadDTO>>
    {
    }

    public class GetAdminCompaniesQueryHandler : IRequestHandler<GetAdminCompaniesQuery, List<CompanyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAdminCompaniesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyReadDTO>> Handle(GetAdminCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .Where(c => c.DeletedAtUtc == null)
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CompanyReadDTO>>(companies);
        }
    }
}
