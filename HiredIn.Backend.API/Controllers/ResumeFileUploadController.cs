using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.ResumeFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class ResumeFileUploadController : BaseController
    {
        [HttpPost("{resumeId:guid}")]
        public async Task<ActionResult<Guid>> UploadResumeFile(
            Guid resumeId,
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
                new UploadResumeFileCommand(
                    resumeId,
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    memoryStream.ToArray(),
                    extension),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{resumeId:guid}")]
        public async Task<ActionResult<string>> GetResumeFileUrl(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetResumeFileUrlQuery(resumeId),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{resumeId:guid}")]
        public async Task<IActionResult> DeleteResumeFile(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new DeleteResumeFileCommand(resumeId),
                cancellationToken);

            return NoContent();
        }
    }
}