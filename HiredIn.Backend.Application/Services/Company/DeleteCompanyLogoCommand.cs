using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class DeleteCompanyLogoCommand : IRequest
    {
        //public Guid CompanyId { get; set; }

        //public DeleteCompanyLogoCommand(Guid companyId)
        //{
        //    CompanyId = companyId;
        //}
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

            var usersCompanyId = await _context.CompanyMembers
                .Where(cm => cm.UserId == currentUserId.Value)
                .Select(cm => cm.CompanyId)
                .FirstOrDefaultAsync(cancellationToken);

            if (usersCompanyId == Guid.Empty)
                throw new KeyNotFoundException("User does not belong to any company.");



            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == usersCompanyId &&
                    c.DeletedAtUtc == null,
                    cancellationToken);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == usersCompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role != CompanyMemberRole.Owner)
                throw new UnauthorizedAccessException("Only company owner can delete company logo.");

            var possiblePaths = new[]
            {
                $"companies/{usersCompanyId}/logo.jpg",
                $"companies/{usersCompanyId}/logo.jpeg",
                $"companies/{usersCompanyId}/logo.png",
                $"companies/{usersCompanyId}/logo.webp"
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