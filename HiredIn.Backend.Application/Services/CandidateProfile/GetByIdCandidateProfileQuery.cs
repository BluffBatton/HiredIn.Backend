using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CandidateProfile
{
    public class GetByIdCandidateProfileQuery : IRequest<CandidateProfileReadDTO>
    {
        public Guid CandidateProfileId { get; set; }
        public GetByIdCandidateProfileQuery(Guid candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }

    public class GetByIdCandidateProfileQueryHandler : IRequestHandler<GetByIdCandidateProfileQuery, CandidateProfileReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetByIdCandidateProfileQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CandidateProfileReadDTO> Handle(GetByIdCandidateProfileQuery request, CancellationToken cancellationToken)
        {
            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(x => x.Id == request.CandidateProfileId, cancellationToken);
            if (candidateProfile is null)
            {
                throw new ArgumentException("Profile was not found");
            }

            return _mapper.Map<CandidateProfileReadDTO>(candidateProfile);
        }
    }
}
