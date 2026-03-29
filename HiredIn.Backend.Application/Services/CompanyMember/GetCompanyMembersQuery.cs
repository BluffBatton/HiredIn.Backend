using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyMember
{
    public class GetCompanyMembersQuery : IRequest<List<CompanyMemberReadDTO>>
    {
    }

    public class GetCompanyMembersQueryHandler : IRequestHandler<GetCompanyMembersQuery, List<CompanyMemberReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetCompanyMembersQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<CompanyMemberReadDTO>> Handle(GetCompanyMembersQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentMembership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm => cm.UserId == currentUserId.Value, cancellationToken);

            if (currentMembership == null)
                throw new UnauthorizedAccessException("You do not belong to any company.");

            var members = await _context.CompanyMembers
                .Where(cm => cm.CompanyId == currentMembership.CompanyId)
                .Include(cm => cm.User)
                .Include(cm => cm.Company)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CompanyMemberReadDTO>>(members);
        }
    }
}