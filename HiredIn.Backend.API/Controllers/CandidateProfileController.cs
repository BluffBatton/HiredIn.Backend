using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Services.CandidateProfile;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using HiredIn.Backend.Contracts.DTOs.Enums;
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

        [HttpGet]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<ActionResult<PaginatedList<CandidateSearchResultDTO>>> SearchCandidates(
            [FromQuery] string? searchText,
            [FromQuery] string? city,
            [FromQuery] bool openToWorkOnly = true,
            [FromQuery] Guid? skillId = null,
            [FromQuery] string? skill = null,
            [FromQuery] EmploymentType? employmentType = null,
            [FromQuery] WorkFormat? workFormat = null,
            [FromQuery] ExperienceLevel? experienceLevel = null,
            [FromQuery] string? sortBy = "updatedAt",
            [FromQuery] string? sortDirection = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new SearchCandidatesQuery
            {
                SearchText = searchText,
                City = city,
                OpenToWorkOnly = openToWorkOnly,
                SkillId = skillId,
                Skill = skill,
                EmploymentType = employmentType,
                WorkFormat = workFormat,
                ExperienceLevel = experienceLevel,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize
            }, cancellationToken);

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
