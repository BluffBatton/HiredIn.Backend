using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeEducation
{
    public class GetResumeEducationsByResumeIdQuery : IRequest<List<ResumeEducationReadDTO>>
    {
        public Guid ResumeId { get; set; }

        public GetResumeEducationsByResumeIdQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeEducationsByResumeIdQueryHandler : IRequestHandler<GetResumeEducationsByResumeIdQuery, List<ResumeEducationReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetResumeEducationsByResumeIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ResumeEducationReadDTO>> Handle(GetResumeEducationsByResumeIdQuery request, CancellationToken cancellationToken)
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

            var educations = await _context.ResumeEducations
                .Where(e => e.ResumeId == request.ResumeId && e.DeletedAtUtc == null)
                .OrderByDescending(e => e.UpdatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ResumeEducationReadDTO>>(educations);
        }
    }
}