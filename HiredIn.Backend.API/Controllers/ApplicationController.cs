using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Application;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    //[Authorize]
    public class ApplicationController : BaseController
    {
        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public async Task<ActionResult<ApplicationReadDTO>> CreateApplication(
            [FromBody] ApplicationCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CreateApplicationCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<ApplicationReadDTO>>> GetMyApplications(
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetMyApplicationsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{applicationId:guid}")]
        public async Task<ActionResult<ApplicationReadDTO>> GetApplicationById(
            Guid applicationId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetApplicationByIdQuery(applicationId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{applicationId:guid}/withdraw")]
        //[Authorize(Roles = "Candidate")]
        public async Task<IActionResult> WithdrawApplication(
            Guid applicationId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(new WithdrawApplicationCommand(applicationId), cancellationToken);
            return NoContent();
        }

        [HttpGet("{vacancyId:guid}/vacancy")]
        //[Authorize(Roles = "Employer")]
        public async Task<ActionResult<List<ApplicationReadDTO>>> GetApplicationsByVacancyId(
            Guid vacancyId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetApplicationsByVacancyIdQuery(vacancyId), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{applicationId:guid}/status")]
        //[Authorize(Roles = "Employer")]
        public async Task<ActionResult<ApplicationReadDTO>> UpdateApplicationStatus(
            Guid applicationId,
            [FromBody] ApplicationStatusUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new UpdateApplicationStatusCommand(applicationId, dto), cancellationToken);
            return Ok(result);
        }
    }
}
