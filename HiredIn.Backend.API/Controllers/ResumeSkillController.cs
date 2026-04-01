using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.ResumeSkill;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/resumeskill")]
    public class ResumeSkillController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<ResumeSkillReadDTO>> Create(
            [FromBody] ResumeSkillCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateResumeSkillCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-resume/{resumeId:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<ResumeSkillReadDTO>>> GetByResumeId(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetResumeSkillsByResumeIdQuery(resumeId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{resumeId:guid}/{skillId:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Patch(
            Guid resumeId,
            Guid skillId,
            [FromBody] ResumeSkillPatchDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new PatchResumeSkillCommand(resumeId, skillId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{resumeId:guid}/{skillId:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Delete(
            Guid resumeId,
            Guid skillId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteResumeSkillCommand(resumeId, skillId), cancellationToken);
            return NoContent();
        }
    }
}