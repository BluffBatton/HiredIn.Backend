using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Resume;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    //[Authorize(Roles="Candidate")]
    public class ResumeController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateResume(
            [FromBody] ResumeCreateDTO resume,
            CancellationToken cancellationToken)
        {
            var command = new CreateResumeCommand(resume);
            var resumeId = await Mediator.Send(command, cancellationToken);

            return Ok(resumeId);
        }

        [HttpGet]
        public async Task<ActionResult<List<ResumeReadDTO>>> GetMyResumes(CancellationToken cancellationToken)
        {
            var resumes = await Mediator.Send(new GetMyResumesQuery(), cancellationToken);

            return Ok(resumes);
        }
    }
}
