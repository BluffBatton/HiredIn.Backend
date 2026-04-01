using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Skill
{
    public class GetSkillByIdQuery : IRequest<SkillReadDTO>
    {
        public Guid Id { get; set; }

        public GetSkillByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, SkillReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetSkillByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SkillReadDTO> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.DeletedAtUtc == null, cancellationToken);

            if (skill == null)
                throw new KeyNotFoundException("Skill not found.");

            return _mapper.Map<SkillReadDTO>(skill);
        }
    }
}