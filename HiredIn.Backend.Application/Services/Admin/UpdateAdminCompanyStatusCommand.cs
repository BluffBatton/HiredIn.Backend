using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.AdminDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class UpdateAdminCompanyStatusCommand : IRequest
    {
        public Guid CompanyId { get; set; }
        public CompanyStatusUpdateDTO Dto { get; set; }

        public UpdateAdminCompanyStatusCommand(Guid companyId, CompanyStatusUpdateDTO dto)
        {
            CompanyId = companyId;
            Dto = dto;
        }
    }

    public class UpdateAdminCompanyStatusCommandHandler : IRequestHandler<UpdateAdminCompanyStatusCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAdminCompanyStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateAdminCompanyStatusCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == request.CompanyId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            company.Status = (Domain.Enums.CompanyStatus)request.Dto.Status;
            company.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
