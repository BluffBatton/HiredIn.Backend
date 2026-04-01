using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainSkill = HiredIn.Backend.Domain.Entities.Skill;

namespace HiredIn.Backend.Application.Services.Skill
{
    public class CreateSkillCommand : IRequest<SkillReadDTO>
    {
        public SkillCreateDTO Skill { get; set; }

        public CreateSkillCommand(SkillCreateDTO skill)
        {
            Skill = skill;
        }
    }

    public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, SkillReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateSkillCommandHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SkillReadDTO> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.Skill.Name.Trim();

            var alreadyExists = await _context.Skills
                .AnyAsync(s => s.Name == normalizedName && s.DeletedAtUtc == null, cancellationToken);

            if (alreadyExists)
                throw new InvalidOperationException("Skill already exists.");

            var skill = _mapper.Map<DomainSkill>(request.Skill);
            skill.Name = normalizedName;
            skill.CreatedAtUtc = DateTime.UtcNow;
            skill.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Skills.AddAsync(skill, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SkillReadDTO>(skill);
        }
    }
}