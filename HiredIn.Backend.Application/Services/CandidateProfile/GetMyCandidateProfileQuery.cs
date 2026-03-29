using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CandidateProfile
{
    public class GetMyCandidateProfileQuery : IRequest<CandidateProfileReadDTO>{}

    public class GetMyCandidateProfileQueryHandler : IRequestHandler<GetMyCandidateProfileQuery, CandidateProfileReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetMyCandidateProfileQueryHandler(IApplicationDbContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<CandidateProfileReadDTO> Handle(GetMyCandidateProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(x => x.User.Id == userId, cancellationToken);
            if (candidateProfile == null)
            {
                throw new UnauthorizedAccessException("Candidate profile not found for the current user.");
            }
            return _mapper.Map<CandidateProfileReadDTO>(candidateProfile);
        }
    }
}
