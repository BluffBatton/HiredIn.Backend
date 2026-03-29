using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CandidateProfile
{
    public class UpdateMyCandidateProfileCommand : IRequest
    {
        public CandidateProfileUpdateDTO Dto { get; set; }
        public UpdateMyCandidateProfileCommand(CandidateProfileUpdateDTO dto)
        {
            Dto = dto;
        }
    }

    public class UpdateMyCandidateProfileCommandHandler : IRequestHandler<UpdateMyCandidateProfileCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public UpdateMyCandidateProfileCommandHandler(IApplicationDbContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(UpdateMyCandidateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

            if (candidateProfile == null)
            {
                throw new UnauthorizedAccessException("Candidate profile was not found");
            }

            _mapper.Map(request.Dto, candidateProfile);

            candidateProfile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
