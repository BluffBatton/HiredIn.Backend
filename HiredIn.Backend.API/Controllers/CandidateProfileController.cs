using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.CandidateProfile;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class CandidateProfileController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<CandidateProfileReadDTO>> GetMyCandidateProfile()
        {
            var query = new GetMyCandidateProfileQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<CandidateProfileReadDTO>> GetCandidateProfileById(Guid Id)
        {
            var query = new GetByIdCandidateProfileQuery(Id);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateProfileReadDTO>>> GetAllCandidateProfiles()
        {
            var query = new GetAllCandidateProfilesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMyCandidateProfile([FromBody] CandidateProfileUpdateDTO dto)
        {
            var command = new UpdateMyCandidateProfileCommand(dto);
            await Mediator.Send(command);
            return Ok();
        }
    }
}
