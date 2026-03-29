using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.CompanyMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize(Roles = "Employer")]
    public class CompanyMemberController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Contracts.DTOs.CompanyMemberDTOs.CompanyMemberCreateDTO companyMemberCreateDTO)
        {
            var command = new CreateCompanyMemberCommand(companyMemberCreateDTO);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyMembers()
        {
            var query = new GetCompanyMembersQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCompanyMemberById(Guid id)
        {
            var query = new GetByIdCompanyMemberQuery(id);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompanyMember(Guid id)
        {
            var command = new DeleteCompanyMemberCommand(id);
            await Mediator.Send(command);
            return Ok();
        }
    }
}
