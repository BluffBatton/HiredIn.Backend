using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Auth
{
    public class RegisterCandidateCommand : IRequest<AuthResponseDto>
    {
        public RegisterCandidateRequestDto Register { get; set; } = null!;
    }

    public class RegisterCandidateCommandHandler : IRequestHandler<RegisterCandidateCommand, AuthResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtService _jwtService;

        public RegisterCandidateCommandHandler(
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

        public async Task<AuthResponseDto> Handle(RegisterCandidateCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Register.Email.Trim().ToLowerInvariant();

            var userExists = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

            if (userExists)
                throw new InvalidOperationException($"User with email {request.Register.Email} already exists");

            var user = _mapper.Map<User>(request.Register);
            user.Email = normalizedEmail;
            user.Role = UserRole.Candidate;
            user.Status = UserStatus.Active;
            user.PasswordHash = _passwordHasherService.HashPassword(user, request.Register.Password);
            user.CreatedAtUtc = DateTime.UtcNow;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var candidateProfile = _mapper.Map<CandidateProfile>(request.Register);
            candidateProfile.UserId = user.Id;
            candidateProfile.OpenToWork = true;
            candidateProfile.CreatedAtUtc = DateTime.UtcNow;
            candidateProfile.CreatedAtUtc = DateTime.UtcNow;
            candidateProfile.CreatedAtUtc = DateTime.UtcNow;
            candidateProfile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.CandidateProfiles.AddAsync(candidateProfile, cancellationToken);
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
