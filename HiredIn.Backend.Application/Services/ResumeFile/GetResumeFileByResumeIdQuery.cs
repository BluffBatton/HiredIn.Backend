using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class GetResumeFileByResumeIdQuery : IRequest<ResumeFileReadDTO>
    {
        public Guid ResumeId { get; set; }

        public GetResumeFileByResumeIdQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeFileByResumeIdQueryHandler : IRequestHandler<GetResumeFileByResumeIdQuery, ResumeFileReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetResumeFileByResumeIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResumeFileReadDTO> Handle(GetResumeFileByResumeIdQuery request, CancellationToken cancellationToken)
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

            var resumeFile = await _context.ResumeFiles
                .FirstOrDefaultAsync(rf =>
                    rf.ResumeId == request.ResumeId &&
                    rf.DeletedAtUtc == null,
                    cancellationToken);

            if (resumeFile == null)
                throw new KeyNotFoundException("Resume file not found.");

            return _mapper.Map<ResumeFileReadDTO>(resumeFile);
        }
    }
}