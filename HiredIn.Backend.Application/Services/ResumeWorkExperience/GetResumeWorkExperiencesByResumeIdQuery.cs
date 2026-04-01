using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeWorkExperience
{
    public class GetResumeWorkExperiencesByResumeIdQuery : IRequest<List<ResumeWorkExperienceReadDTO>>
    {
        public Guid ResumeId { get; set; }

        public GetResumeWorkExperiencesByResumeIdQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeWorkExperiencesByResumeIdQueryHandler : IRequestHandler<GetResumeWorkExperiencesByResumeIdQuery, List<ResumeWorkExperienceReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetResumeWorkExperiencesByResumeIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ResumeWorkExperienceReadDTO>> Handle(GetResumeWorkExperiencesByResumeIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resume = await _context.Resumes
                .Include(r => r.CandidateProfile)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume.");

            var workExperiences = await _context.ResumeWorkExperiences
                .Where(w => w.ResumeId == request.ResumeId)
                .OrderByDescending(w => w.IsCurrent)
                .ThenByDescending(w => w.EndDate)
                .ThenByDescending(w => w.StartDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ResumeWorkExperienceReadDTO>>(workExperiences);
        }
    }
}