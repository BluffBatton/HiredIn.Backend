using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Vacancy;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Route("api/vacancy")]
    public class VacancyController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] VacancyCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateVacancyCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<List<VacancyReadDTO>>> GetMy(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetMyVacanciesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{vacancyId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<VacancyReadDTO>> GetById(
            Guid vacancyId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetVacancyByIdQuery(vacancyId), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{vacancyId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Update(
            Guid vacancyId,
            [FromBody] VacancyUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new UpdateVacancyCommand(vacancyId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{vacancyId:guid}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(
            Guid vacancyId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteVacancyCommand(vacancyId), cancellationToken);
            return NoContent();
        }
    }
}