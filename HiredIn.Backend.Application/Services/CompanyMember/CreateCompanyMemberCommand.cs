using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyMember
{
    public class CreateCompanyMemberCommand : IRequest<Guid>
    {
        public CompanyMemberCreateDTO CompanyMember { get; set; }

        public CreateCompanyMemberCommand(CompanyMemberCreateDTO companyMember)
        {
            CompanyMember = companyMember;
        }
    }

    public class CreateCompanyMemberCommandHandler : IRequestHandler<CreateCompanyMemberCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateCompanyMemberCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateCompanyMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentMembership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm => cm.UserId == currentUserId.Value, cancellationToken);

            if (currentMembership == null)
                throw new UnauthorizedAccessException("You do not belong to any company.");

            if (currentMembership.Role != CompanyMemberRole.Owner)
                throw new UnauthorizedAccessException("You do not have permission to add company members.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.CompanyMember.Email.Trim().ToLower(), cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var alreadyExists = await _context.CompanyMembers
                .AnyAsync(
                    cm => cm.CompanyId == currentMembership.CompanyId &&
                          cm.UserId == user.Id,
                    cancellationToken);

            if (alreadyExists)
                throw new InvalidOperationException("User is already a member of this company.");

            var companyMember = new Domain.Entities.CompanyMember
            {
                CompanyId = currentMembership.CompanyId,
                UserId = user.Id,
                Role = (HiredIn.Backend.Domain.Enums.CompanyMemberRole)request.CompanyMember.Role,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.CompanyMembers.AddAsync(companyMember, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return companyMember.Id;
        }
    }
}