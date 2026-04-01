using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeEducation
{
    public class GetResumeEducationByIdQuery : IRequest<ResumeEducationReadDTO>
    {
        public Guid Id { get; set; }

        public GetResumeEducationByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetResumeEducationByIdQueryHandler : IRequestHandler<GetResumeEducationByIdQuery, ResumeEducationReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetResumeEducationByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResumeEducationReadDTO> Handle(GetResumeEducationByIdQuery request, CancellationToken cancellationToken)
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

            return _mapper.Map<ResumeEducationReadDTO>(education);
        }
    }
}