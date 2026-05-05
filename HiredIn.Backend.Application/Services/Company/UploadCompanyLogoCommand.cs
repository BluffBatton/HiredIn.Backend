using HiredIn.Backend.Application.Common.Files;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class UploadCompanyLogoCommand : IRequest<string>
    {
        public Guid CompanyId { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public byte[] FileBytes { get; set; }
        public string Extension { get; set; }

        public UploadCompanyLogoCommand(
            Guid companyId,
            string contentType,
            long size,
            byte[] fileBytes,
            string extension)
        {
            CompanyId = companyId;
            ContentType = contentType;
            Size = size;
            FileBytes = fileBytes;
            Extension = extension;
        }
    }

    public class UploadCompanyLogoCommandHandler : IRequestHandler<UploadCompanyLogoCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public UploadCompanyLogoCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task<string> Handle(UploadCompanyLogoCommand request, CancellationToken cancellationToken)
        {
            FileValidationHelper.ValidateImage(request.ContentType, request.Size);

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
                throw new UnauthorizedAccessException("Only company owner can upload company logo.");

            var path = $"companies/{request.CompanyId}/logo{request.Extension}";

            await _fileStorageService.UploadAsync(
                "company-logos",
                path,
                request.FileBytes,
                cancellationToken);

            var publicUrl = _fileStorageService.GetPublicUrl("company-logos", path);

            company.LogoUrl = publicUrl;
            company.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return publicUrl;
        }
    }
}