using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.ResumeFile;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/resumefile")]
    public class ResumeFileController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] ResumeFileCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateResumeFileCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-resume/{resumeId:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<ResumeFileReadDTO>> GetByResumeId(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetResumeFileByResumeIdQuery(resumeId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Patch(
            Guid id,
            [FromBody] ResumeFilePatchDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new PatchResumeFileCommand(id, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteResumeFileCommand(id), cancellationToken);
            return NoContent();
        }
    }
}