using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AdminDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class UpdateAdminVacancyStatusCommand : IRequest
    {
        public Guid VacancyId { get; set; }
        public VacancyStatusUpdateDTO Dto { get; set; }

        public UpdateAdminVacancyStatusCommand(Guid vacancyId, VacancyStatusUpdateDTO dto)
        {
            VacancyId = vacancyId;
            Dto = dto;
        }
    }

    public class UpdateAdminVacancyStatusCommandHandler : IRequestHandler<UpdateAdminVacancyStatusCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAdminVacancyStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateAdminVacancyStatusCommand request, CancellationToken cancellationToken)
        {
            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            vacancy.Status = (Domain.Enums.VacancyStatus)request.Dto.Status;
            vacancy.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
