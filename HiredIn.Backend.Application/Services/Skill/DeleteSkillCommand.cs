using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Skill
{
    public class DeleteSkillCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteSkillCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSkillCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.DeletedAtUtc == null, cancellationToken);

            if (skill == null)
                throw new KeyNotFoundException("Skill not found.");

            skill.DeletedAtUtc = DateTime.UtcNow;
            skill.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}