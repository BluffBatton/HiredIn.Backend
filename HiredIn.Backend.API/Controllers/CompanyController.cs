using HiredIn.Backend.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiredIn.Backend.Application.Services.Company;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize(Roles = "Employer")]
    public class CompanyController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] Contracts.DTOs.CompanyDTOs.CompanyCreateDTO companyCreateDTO)
        {
            var command = new CreateCompanyCommand(companyCreateDTO);
            await Mediator.Send(command);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<CompanyReadDTO>> GetMyCompany()
        {
            var query = new GetMyCompanyQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<CompanyReadDTO>>> GetAllCompanies()
        {
            var query = new GetAllCompaniesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateMyCompany([FromBody] CompanyUpdateDTO companyUpdateDTO)
        {
            var command = new UpdateMyCompanyCommand(companyUpdateDTO);
            await Mediator.Send(command);
            return Ok();
        }
    }
}