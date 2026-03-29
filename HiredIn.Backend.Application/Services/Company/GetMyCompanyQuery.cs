using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class GetMyCompanyQuery : IRequest<CompanyReadDTO>
    {

    }

    public class GetMyCompanyQueryHandler : IRequestHandler<GetMyCompanyQuery, CompanyReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public GetMyCompanyQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public async Task<CompanyReadDTO> Handle(GetMyCompanyQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");
            var companyMember = await _context.CompanyMembers
                .Include(cm => cm.Company)
                .FirstOrDefaultAsync(cm => cm.UserId == currentUserId.Value, cancellationToken);
            if (companyMember == null || companyMember.Company == null)
                throw new KeyNotFoundException("Company not found for the current user.");
            return _mapper.Map<CompanyReadDTO>(companyMember.Company);
        }
    }
}
