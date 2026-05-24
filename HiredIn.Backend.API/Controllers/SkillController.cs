using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Skill;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/skill")]
    [Authorize]

    public class SkillController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SkillReadDTO>> Create(
            [FromBody] SkillCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateSkillCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SkillReadDTO>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetSkillByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillReadDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetSkillsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Patch(
            Guid id,
            [FromBody] SkillPatchDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new PatchSkillCommand(id, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteSkillCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
