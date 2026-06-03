using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Application
{
    public class GetApplicationByIdQuery : IRequest<ApplicationReadDTO>
    {
        public Guid ApplicationId { get; set; }

        public GetApplicationByIdQuery(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }

    public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, ApplicationReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetApplicationByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ApplicationReadDTO> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var application = await _context.Applications
                .Include(a => a.Resume)
                    .ThenInclude(r => r.CandidateProfile)
                        .ThenInclude(cp => cp.User)
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.Company)
                .FirstOrDefaultAsync(a =>
                    a.Id == request.ApplicationId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            var isCandidateOwner = application.Resume.CandidateProfile.UserId == currentUserId.Value;

            var isEmployerOwner = await _context.CompanyMembers
                .AnyAsync(cm =>
                    cm.CompanyId == application.Vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (!isCandidateOwner && !isEmployerOwner)
                throw new UnauthorizedAccessException("You do not have access to this application.");

            return _mapper.Map<ApplicationReadDTO>(application);
        }
    }
}
