using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class DeleteAdminVacancyCommand : IRequest
    {
        public Guid VacancyId { get; set; }

        public DeleteAdminVacancyCommand(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }

    public class DeleteAdminVacancyCommandHandler : IRequestHandler<DeleteAdminVacancyCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAdminVacancyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAdminVacancyCommand request, CancellationToken cancellationToken)
        {
            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            vacancy.Status = VacancyStatus.Archived;
            vacancy.DeletedAtUtc = DateTime.UtcNow;
            vacancy.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
