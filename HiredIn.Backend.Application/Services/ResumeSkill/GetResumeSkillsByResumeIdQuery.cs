using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeSkill
{
    public class GetResumeSkillsByResumeIdQuery : IRequest<List<ResumeSkillReadDTO>>
    {
        public Guid ResumeId { get; set; }

        public GetResumeSkillsByResumeIdQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeSkillsByResumeIdQueryHandler : IRequestHandler<GetResumeSkillsByResumeIdQuery, List<ResumeSkillReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetResumeSkillsByResumeIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<ResumeSkillReadDTO>> Handle(GetResumeSkillsByResumeIdQuery request, CancellationToken cancellationToken)
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

            var resumeSkills = await _context.ResumeSkills
                .Where(rs => rs.ResumeId == request.ResumeId)
                .Include(rs => rs.Skill)
                .OrderBy(rs => rs.Skill.Name)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ResumeSkillReadDTO>>(resumeSkills);
        }
    }
}