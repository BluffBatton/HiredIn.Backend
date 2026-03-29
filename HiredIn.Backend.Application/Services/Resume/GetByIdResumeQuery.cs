using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class GetByIdResumeQuery : IRequest<ResumeReadDTO>
    {
        public Guid ResumeId { get; set; }

        public GetByIdResumeQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeByIdQueryHandler : IRequestHandler<GetByIdResumeQuery, ResumeReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetResumeByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResumeReadDTO> Handle(GetByIdResumeQuery request, CancellationToken cancellationToken)
        {
            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r => r.Id == request.ResumeId, cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            return _mapper.Map<ResumeReadDTO>(resume);
        }
    }
}