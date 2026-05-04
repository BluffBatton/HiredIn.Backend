using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.CompanyRating;
using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class CompanyRatingController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCompanyRating(
            [FromBody] CompanyRatingCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var id = await Mediator.Send(new CreateCompanyRatingCommand(dto), cancellationToken);
            return Ok(id);
        }

        [HttpGet("{companyRatingId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyRatingReadDTO>> GetCompanyRatingById(
            Guid companyRatingId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetCompanyRatingByIdQuery(companyRatingId),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("by-company/{companyId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CompanyRatingReadDTO>>> GetCompanyRatingsByCompanyId(
            Guid companyId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetCompanyRatingsByCompanyIdQuery(companyId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{companyRatingId:guid}")]
        public async Task<IActionResult> UpdateCompanyRating(
            Guid companyRatingId,
            [FromBody] CompanyRatingUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new UpdateCompanyRatingCommand(companyRatingId, dto),
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{companyRatingId:guid}")]
        public async Task<IActionResult> DeleteCompanyRating(
            Guid companyRatingId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new DeleteCompanyRatingCommand(companyRatingId),
                cancellationToken);

            return NoContent();
        }
    }
}