using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Recommendation;
using HiredIn.Backend.Contracts.DTOs.RecommendationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class RecommendationController : BaseController
    {
        [HttpGet("{resumeId:guid}")]
        public async Task<ActionResult<List<VacancyRecommendationReadDTO>>> GetRecommendedVacancies(
            Guid resumeId,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(
                new GetRecommendedVacanciesQuery(resumeId, limit),
                cancellationToken);

            return Ok(result);
        }
    }
}