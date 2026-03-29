using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class GetMyResumesQuery : IRequest<List<ResumeReadDTO>>
    {
    }

    public class GetMyResumesQueryHandler : IRequestHandler<GetMyResumesQuery, List<ResumeReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetMyResumesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ResumeReadDTO>> Handle(GetMyResumesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var resumes = await _context.Resumes
                .Where(r => r.CandidateProfileId == candidateProfile.Id)
                .OrderByDescending(r => r.IsPrimary)
                .ThenByDescending(r => r.UpdatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ResumeReadDTO>>(resumes);
        }
    }
}