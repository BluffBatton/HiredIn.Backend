using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainResumeEducation = HiredIn.Backend.Domain.Entities.ResumeEducation;

namespace HiredIn.Backend.Application.Services.ResumeEducation
{
    public class PatchResumeEducationCommand : IRequest
    {
        public Guid Id { get; set; }
        public ResumeEducationPatchDTO Education { get; set; }

        public PatchResumeEducationCommand(Guid id, ResumeEducationPatchDTO education)
        {
            Id = id;
            Education = education;
        }
    }

    public class PatchResumeEducationCommandHandler : IRequestHandler<PatchResumeEducationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public PatchResumeEducationCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchResumeEducationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var education = await _context.ResumeEducations
                .Include(e => e.Resume)
                .ThenInclude(r => r.CandidateProfile)
                .FirstOrDefaultAsync(e =>
                    e.Id == request.Id &&
                    e.DeletedAtUtc == null,
                    cancellationToken);

            if (education == null)
                throw new KeyNotFoundException("Resume education not found.");

            if (education.Resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume education.");

            _mapper.Map(request.Education, education);
            education.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}