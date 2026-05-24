using HiredIn.Backend.Application.Services.Admin;
using HiredIn.Backend.Contracts.DTOs.AdminDTOs;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using HiredIn.Backend.Contracts.DTOs.UserDTOs;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserReadDTO>>> GetUsers(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminUsersQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("users/{userId:guid}/status")]
        public async Task<IActionResult> UpdateUserStatus(
            Guid userId,
            [FromBody] UserStatusUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateAdminUserStatusCommand(userId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("users/{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAdminUserCommand(userId), cancellationToken);
            return NoContent();
        }

        [HttpGet("companies")]
        public async Task<ActionResult<List<CompanyReadDTO>>> GetCompanies(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminCompaniesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("companies/{companyId:guid}/status")]
        public async Task<IActionResult> UpdateCompanyStatus(
            Guid companyId,
            [FromBody] CompanyStatusUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateAdminCompanyStatusCommand(companyId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("companies/{companyId:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAdminCompanyCommand(companyId), cancellationToken);
            return NoContent();
        }

        [HttpGet("vacancies")]
        public async Task<ActionResult<List<VacancyReadDTO>>> GetVacancies(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminVacanciesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPatch("vacancies/{vacancyId:guid}/status")]
        public async Task<IActionResult> UpdateVacancyStatus(
            Guid vacancyId,
            [FromBody] VacancyStatusUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateAdminVacancyStatusCommand(vacancyId, dto), cancellationToken);
            return NoContent();
        }

        [HttpDelete("vacancies/{vacancyId:guid}")]
        public async Task<IActionResult> DeleteVacancy(Guid vacancyId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAdminVacancyCommand(vacancyId), cancellationToken);
            return NoContent();
        }

        [HttpGet("company-ratings")]
        public async Task<ActionResult<List<CompanyRatingReadDTO>>> GetCompanyRatings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminCompanyRatingsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("company-ratings/{ratingId:guid}")]
        public async Task<IActionResult> DeleteCompanyRating(Guid ratingId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAdminCompanyRatingCommand(ratingId), cancellationToken);
            return NoContent();
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<AdminStatisticsDTO>> GetStatistics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminStatisticsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
