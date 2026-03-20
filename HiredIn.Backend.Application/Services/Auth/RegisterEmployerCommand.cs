using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using HiredIn.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HiredIn.Backend.Domain.Enums;
using MediatR;

namespace HiredIn.Backend.Application.Services.Auth
{
    public class RegisterEmployerCommand : IRequest<AuthResponseDto>
    {
        public RegisterEmployerRequestDto Register { get; set; } = null!;
    }

    public class RegisterEmployerCommandHandler : IRequestHandler<RegisterEmployerCommand, AuthResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtService _jwtService;

        public RegisterEmployerCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IPasswordHasherService passwordHasherService,
            IJwtService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasherService = passwordHasherService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Register.Email.Trim().ToLowerInvariant();

            var userExists = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

            if (userExists)
                throw new InvalidOperationException($"User with email {request.Register.Email} already exists");

            var user = _mapper.Map<User>(request.Register);
            user.Email = normalizedEmail;
            user.Role = UserRole.Employer;
            user.Status = UserStatus.Active;
            user.PasswordHash = _passwordHasherService.HashPassword(user, request.Register.Password);
            user.CreatedAtUtc = DateTime.UtcNow;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Users.AddAsync(user, cancellationToken);
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
