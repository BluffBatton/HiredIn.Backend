using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.VacancySkill;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/vacancyskill")]
    public class VacancySkillController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<VacancySkillReadDTO>> Create(
            [FromBody] VacancySkillCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateVacancySkillCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-vacancy/{vacancyId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<List<VacancySkillReadDTO>>> GetByVacancyId(
            Guid vacancyId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetVacancySkillsByVacancyIdQuery(vacancyId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{vacancyId:guid}/{skillId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Patch(
            Guid vacancyId,
            Guid skillId,
            [FromBody] VacancySkillPatchDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new PatchVacancySkillCommand(vacancyId, skillId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{vacancyId:guid}/{skillId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(
            Guid vacancyId,
            Guid skillId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteVacancySkillCommand(vacancyId, skillId), cancellationToken);
            return NoContent();
        }
    }
}