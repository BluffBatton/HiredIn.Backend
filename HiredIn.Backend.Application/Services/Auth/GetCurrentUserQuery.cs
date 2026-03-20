using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Auth
{
    public class GetCurrentUserQuery : IRequest<UserInfoDto>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserInfoDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetCurrentUserQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<UserInfoDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            return new UserInfoDto
            {
                ID = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = (Contracts.DTOs.Enums.UserRole)user.Role
            };
        }
    }
}
