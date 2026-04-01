using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainResumeSkill = HiredIn.Backend.Domain.Entities.ResumeSkill;

namespace HiredIn.Backend.Application.Services.ResumeSkill
{
    public class CreateResumeSkillCommand : IRequest<ResumeSkillReadDTO>
    {
        public ResumeSkillCreateDTO ResumeSkill { get; set; }

        public CreateResumeSkillCommand(ResumeSkillCreateDTO resumeSkill)
        {
            ResumeSkill = resumeSkill;
        }
    }

    public class CreateResumeSkillCommandHandler : IRequestHandler<CreateResumeSkillCommand, ResumeSkillReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateResumeSkillCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResumeSkillReadDTO> Handle(CreateResumeSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeSkill.ResumeId &&
                    r.CandidateProfileId == candidateProfile.Id &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.ResumeSkill.SkillId, cancellationToken);

            if (skill == null)
                throw new KeyNotFoundException("Skill not found.");

            var alreadyExists = await _context.ResumeSkills
                .AnyAsync(rs =>
                    rs.ResumeId == request.ResumeSkill.ResumeId &&
                    rs.SkillId == request.ResumeSkill.SkillId,
                    cancellationToken);

            if (alreadyExists)
                throw new InvalidOperationException("This skill is already added to the resume.");

            var resumeSkill = _mapper.Map<DomainResumeSkill>(request.ResumeSkill);
            resumeSkill.Resume = resume;
            resumeSkill.Skill = skill;

            await _context.ResumeSkills.AddAsync(resumeSkill, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ResumeSkillReadDTO>(resumeSkill);
        }
    }
}