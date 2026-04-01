using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Skill
{
    public class GetSkillsQuery : IRequest<List<SkillReadDTO>>
    {
    }

    public class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, List<SkillReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetSkillsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SkillReadDTO>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            var skills = await _context.Skills
                .Where(s => s.DeletedAtUtc == null)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SkillReadDTO>>(skills);
        }
    }
}