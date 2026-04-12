using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Services.FavouriteVacancy;
using HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class FavouriteVacancyController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateFavouriteVacancy([FromBody] FavouriteVacancyCreateDTO dto)
        {
            var command = new CreateFavouriteVacancyCommand(dto);
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FavouriteVacancyReadDTO>> GetFavouriteVacancyById([FromBody] Guid id)
        {
            var query = new GetFavouriteVacancyByIdQuery(id);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedList<FavouriteVacancyReadDTO>>> GetPagedFavouriteVacancies([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPagedFavouriteVacancyQuery { Page = page, PageSize = pageSize };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFavouriteVacancy([FromBody] Guid id)
        {
            var command = new DeleteFavouriteVacancyCommand(id);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
