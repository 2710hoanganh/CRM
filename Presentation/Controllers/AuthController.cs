using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.User.Command;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.User;
using Application.Features.User.Query;

namespace Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand { Request = request };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new Response<RegisterModelResponse>(result.Result)
            {
                Data = result.Data,
                Message = result.Message ?? (result.Result == ResponseResult.SUCCESS ? "User registered successfully" : "User registered failed"),
                Errors = result.Errors
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var query = new LoginQuery { Request = request };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new Response<LoginResponse>(result.Result)
            {
                Data = result.Data,
                Message = result.Message ?? (result.Result == ResponseResult.SUCCESS ? "Login successful" : "Login failed"),
                Errors = result.Errors
            });
        }
    }
}