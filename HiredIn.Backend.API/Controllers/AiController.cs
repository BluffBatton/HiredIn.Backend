using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class AiController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<string>> ExplainVacancyRecommendation(
            Guid resumeId,
            Guid vacancyId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new ExplainVacancyRecommendationQuery(resumeId, vacancyId),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{resumeId:guid}")]
        public async Task<ActionResult<string>> GetResumeImprovementTips(
            Guid resumeId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetResumeImprovementTipsQuery(resumeId),
                cancellationToken);

            return Ok(result);
        }
    }
}