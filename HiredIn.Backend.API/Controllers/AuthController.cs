using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Auth;
using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    public class AuthController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> RegisterCandidate(
            [FromBody] RegisterCandidateRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new RegisterCandidateCommand
            {
                Register = request
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> RegisterEmployer(
            [FromBody] RegisterEmployerRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new RegisterEmployerCommand
            {
                Register = request
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new LoginCommand
            {
                LoginDto = request
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> Me(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
