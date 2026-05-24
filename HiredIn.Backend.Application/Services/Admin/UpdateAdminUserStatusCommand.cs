using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AdminDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class UpdateAdminUserStatusCommand : IRequest
    {
        public Guid UserId { get; set; }
        public UserStatusUpdateDTO Dto { get; set; }

        public UpdateAdminUserStatusCommand(Guid userId, UserStatusUpdateDTO dto)
        {
            UserId = userId;
            Dto = dto;
        }
    }

    public class UpdateAdminUserStatusCommandHandler : IRequestHandler<UpdateAdminUserStatusCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAdminUserStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateAdminUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.Status = (Domain.Enums.UserStatus)request.Dto.Status;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
