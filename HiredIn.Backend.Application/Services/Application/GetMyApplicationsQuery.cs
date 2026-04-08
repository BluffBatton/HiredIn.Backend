using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiredIn.Backend.Application.Services.Application
{
    public class GetMyApplicationsQuery : IRequest<List<ApplicationReadDTO>>
    {
    }

    public class GetMyApplicationsQueryHandler : IRequestHandler<GetMyApplicationsQuery, List<ApplicationReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetMyApplicationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ApplicationReadDTO>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var applications = await _context.Applications
                .Where(a => a.Resume.CandidateProfileId == candidateProfile.Id && a.DeletedAtUtc == null)
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.Company)
                .Include(a => a.Resume)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ApplicationReadDTO>>(applications);
        }
    }
}
