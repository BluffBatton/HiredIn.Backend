using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class CompanyFileController : BaseController
    {
        [HttpPost("{companyId:guid}")]
        public async Task<ActionResult<string>> UploadCompanyLogo(
            Guid companyId,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            var extension = Path.GetExtension(file.FileName).ToLower();

            await using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream, cancellationToken);

            var result = await Mediator.Send(
                new UploadCompanyLogoCommand(
                    companyId,
                    file.ContentType,
                    file.Length,
                    memoryStream.ToArray(),
                    extension),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCompanyLogo(
            Guid companyId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new DeleteCompanyLogoCommand(),
                cancellationToken);

            return NoContent();
        }
    }
}