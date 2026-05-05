using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class UserFileController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<string>> UploadMyAvatar(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            await using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            var result = await Mediator.Send(
                new UploadMyAvatarCommand(
                    file.ContentType,
                    file.Length,
                    memoryStream.ToArray()),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMyAvatar(CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteMyAvatarCommand(), cancellationToken);
            return NoContent();
        }
    }
}