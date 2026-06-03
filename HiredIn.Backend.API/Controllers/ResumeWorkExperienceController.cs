using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.ResumeWorkExperience;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/resumeworkexperience")]
    public class ResumeWorkExperienceController : BaseController
    {
        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public async Task<ActionResult<ResumeWorkExperienceReadDTO>> Create(
            [FromBody] ResumeWorkExperienceCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateResumeWorkExperienceCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-resume/{resumeId:guid}")]
        //[Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<ResumeWorkExperienceReadDTO>>> GetByResumeId(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetResumeWorkExperiencesByResumeIdQuery(resumeId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch]
        //[Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Patch(
            [FromBody] ResumeWorkExperiencePatchDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new PatchResumeWorkExperienceCommand(dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        //[Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Delete(
            [FromBody] ResumeWorkExperienceDeleteDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteResumeWorkExperienceCommand(dto), cancellationToken);
            return NoContent();
        }
    }
}