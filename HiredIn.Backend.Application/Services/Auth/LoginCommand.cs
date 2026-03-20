using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Auth
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public LoginRequestDto LoginDto { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasherService _passwordHasherService;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IJwtService jwtService,
            IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.LoginDto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("User is not active");

            var isValidPassword = _passwordHasherService.VerifyPassword(
                user,
                request.LoginDto.Password,
                user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Invalid email or password");

            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = _jwtService.GenerateAccessToken(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserInfoDto
                {
                    ID = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = (Contracts.DTOs.Enums.UserRole)user.Role
                }
            };
        }
    }
}
