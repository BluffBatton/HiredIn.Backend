using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeEducation
{
    public class CreateResumeEducationCommand : IRequest<Guid>
    {
        public ResumeEducationCreateDTO Education { get; set; }

        public CreateResumeEducationCommand(ResumeEducationCreateDTO education)
        {
            Education = education;
        }
    }

    public class CreateResumeEducationCommandHandler : IRequestHandler<CreateResumeEducationCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateResumeEducationCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateResumeEducationCommand request, CancellationToken cancellationToken)
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
                    r.Id == request.Education.ResumeId &&
                    r.CandidateProfileId == candidateProfile.Id &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            var education = _mapper.Map<Domain.Entities.ResumeEducation>(request.Education);
            education.CreatedAtUtc = DateTime.UtcNow;
            education.UpdatedAtUtc = DateTime.UtcNow;

            await _context.ResumeEducations.AddAsync(education, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return education.Id;
        }
    }
}