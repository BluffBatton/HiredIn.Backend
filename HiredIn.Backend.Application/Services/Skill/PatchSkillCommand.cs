using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Skill
{
    public class PatchSkillCommand : IRequest
    {
        public Guid Id { get; set; }
        public SkillPatchDTO Skill { get; set; }

        public PatchSkillCommand(Guid id, SkillPatchDTO skill)
        {
            Id = id;
            Skill = skill;
        }
    }

    public class PatchSkillCommandHandler : IRequestHandler<PatchSkillCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PatchSkillCommandHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(PatchSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.DeletedAtUtc == null, cancellationToken);

            if (skill == null)
                throw new KeyNotFoundException("Skill not found.");

            if (!string.IsNullOrWhiteSpace(request.Skill.Name))
            {
                var normalizedName = request.Skill.Name.Trim();

                var duplicateExists = await _context.Skills
                    .AnyAsync(s =>
                        s.Id != skill.Id &&
                        s.Name == normalizedName &&
                        s.DeletedAtUtc == null,
                        cancellationToken);

                if (duplicateExists)
                    throw new InvalidOperationException("Skill with this name already exists.");
            }

            _mapper.Map(request.Skill, skill);
            skill.Name = skill.Name.Trim();
            skill.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}