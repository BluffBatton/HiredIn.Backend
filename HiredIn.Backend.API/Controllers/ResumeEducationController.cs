using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.ResumeEducation;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class ResumeEducationController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] ResumeEducationCreateDTO dto)
        {
            var result = await Mediator.Send(new CreateResumeEducationCommand(dto));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResumeEducationReadDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetResumeEducationByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-resume/{resumeId:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<ResumeEducationReadDTO>>> GetByResumeId(
            Guid resumeId)
        {
            var result = await Mediator.Send(new GetResumeEducationsByResumeIdQuery(resumeId));
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Patch(
            Guid id,
            [FromBody] ResumeEducationPatchDTO dto)
        {
            await Mediator.Send(new PatchResumeEducationCommand(id, dto));
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteResumeEducationCommand(id));
            return NoContent();
        }
    }
}