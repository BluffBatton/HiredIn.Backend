using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class DeleteCompanyLogoCommand : IRequest
    {
        public Guid CompanyId { get; set; }

        public DeleteCompanyLogoCommand(Guid companyId)
        {
            CompanyId = companyId;
        }
    }

    public class DeleteCompanyLogoCommandHandler : IRequestHandler<DeleteCompanyLogoCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public DeleteCompanyLogoCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteCompanyLogoCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == request.CompanyId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == request.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role != CompanyMemberRole.Owner)
                throw new UnauthorizedAccessException("Only company owner can delete company logo.");

            var possiblePaths = new[]
            {
                $"companies/{request.CompanyId}/logo.jpg",
                $"companies/{request.CompanyId}/logo.jpeg",
                $"companies/{request.CompanyId}/logo.png",
                $"companies/{request.CompanyId}/logo.webp"
            };

            foreach (var path in possiblePaths)
            {
                await _fileStorageService.DeleteAsync("company-logos", path, cancellationToken);
            }

            company.LogoUrl = null;
            company.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}