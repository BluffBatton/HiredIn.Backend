using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyMember
{
    public class GetByIdCompanyMemberQuery : IRequest<CompanyMemberReadDTO>
    {
        public Guid Id { get; set; }

        public GetByIdCompanyMemberQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetByIdCompanyMemberQueryHandler : IRequestHandler<GetByIdCompanyMemberQuery, CompanyMemberReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetByIdCompanyMemberQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<CompanyMemberReadDTO> Handle(GetByIdCompanyMemberQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentMembership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm => cm.UserId == currentUserId.Value, cancellationToken);

            if (currentMembership == null)
                throw new UnauthorizedAccessException("You do not belong to any company.");

            var member = await _context.CompanyMembers
                .Include(cm => cm.User)
                .Include(cm => cm.Company)
                .FirstOrDefaultAsync(cm => cm.Id == request.Id, cancellationToken);

            if (member == null)
                throw new KeyNotFoundException("Company member not found.");

            if (member.CompanyId != currentMembership.CompanyId)
                throw new UnauthorizedAccessException("You do not have access to this company member.");

            return _mapper.Map<CompanyMemberReadDTO>(member);
        }
    }
}