using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class GetByUsersIdResumesQuery : IRequest<List<ResumeReadDTO>>
    {
        public Guid UserId { get; set; }

        public GetByUsersIdResumesQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetByUsersIdResumesHandler : IRequestHandler<GetByUsersIdResumesQuery, List<ResumeReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetByUsersIdResumesHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ResumeReadDTO>> Handle(GetByUsersIdResumesQuery request, CancellationToken cancellationToken)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == request.UserId, cancellationToken);

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