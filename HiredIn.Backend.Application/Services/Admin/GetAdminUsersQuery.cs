using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.UserDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class GetAdminUsersQuery : IRequest<List<UserReadDTO>>
    {
    }

    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<UserReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAdminUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserReadDTO>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.DeletedAtUtc == null)
                .OrderByDescending(u => u.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<UserReadDTO>>(users);
        }
    }
}
