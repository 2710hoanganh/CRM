using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.UserReference;
using System.Security.Claims;
using Application.Features.UserReference.Command;
using Application.Features.UserReference.Query;
using Presentation.DTOs;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user-reference")]
    public class UserReferenceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserReferenceController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateUserReference([FromBody] CreateUserReferenceCommand command, CancellationToken cancellationToken)
        {
            command.Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new Response<bool>(result.Result)
            {
                Data = result.Data,
                Message = result.Message ?? (result.Result == ResponseResult.SUCCESS ? "User references created successfully" : Domain.Constants.Error.UserReferencesCreateFailed),
                Errors = result.Errors
            });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUserReferences([FromQuery] BasePaginationQueryDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserReferenceQuery
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);
            var isSuccess = result.Message != ResponseResult.ERROR.ToString();
            return Ok(new Response<Paged<List<GetUserReferenceResponse>>>(isSuccess ? ResponseResult.SUCCESS : ResponseResult.ERROR)
            {
                Data = result,
                Message = isSuccess ? result.Message : Domain.Constants.Error.GetUserReferencesFailed
            });
        }

    }
}