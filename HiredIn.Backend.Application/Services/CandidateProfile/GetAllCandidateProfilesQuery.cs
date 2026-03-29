using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CandidateProfile
{
    public class GetAllCandidateProfilesQuery : IRequest<List<CandidateProfileReadDTO>>
    {
    }

    public class GetAllCandidateProfilesQueryHandler : IRequestHandler<GetAllCandidateProfilesQuery, List<CandidateProfileReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllCandidateProfilesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CandidateProfileReadDTO>> Handle(GetAllCandidateProfilesQuery request, CancellationToken cancellationToken)
        {
            var candidateProfiles = await _context.CandidateProfiles.ToListAsync(cancellationToken);
            return _mapper.Map<List<CandidateProfileReadDTO>>(candidateProfiles);
        }
    }
}
