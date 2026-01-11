using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.User;
using Application.Features.User.Query;  
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _mediator.Send(new GetUserInfoQuery
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0")
            });
            return Ok(new Response<UserInfo>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Get user info successful" : "Get user info failed"
            });
        }
    }
}