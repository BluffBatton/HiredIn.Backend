using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Resume;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize(Roles="Candidate")]
    public class ResumeController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateResume([FromBody] ResumeCreateDTO resume)
        {
            var command = new CreateResumeCommand(resume);
            await Mediator.Send(command);
            return Ok();
        }
    }
}
