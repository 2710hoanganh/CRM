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
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new Response<RegisterModelResponse>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "User registered successfully" : "User registered failed"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new Response<LoginResponse>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Login successful" : "Login failed"
            });
        }
    }
}