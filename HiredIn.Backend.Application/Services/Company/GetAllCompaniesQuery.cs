using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class GetAllCompaniesQuery : IRequest<List<CompanyReadDTO>>
    {
    }

    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, List<CompanyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCompaniesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyReadDTO>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .Where(c =>
                    c.DeletedAtUtc == null &&
                    c.Status == CompanyStatus.Active)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CompanyReadDTO>>(companies);
        }
    }
}
