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
        public async Task<IActionResult> CreateUserReference([FromBody] CreateUserReferenceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateUserReferenceCommand
            {
                Requests = new List<CreateUserReferenceRequest> { request },
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0")
            }, cancellationToken);
            return Ok(new Response<bool>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "User references created successfully" : "User references created failed"
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
            return Ok(new Response<Paged<List<GetUserReferenceResponse>>>(ResponseResult.SUCCESS)
            {
                Data = result,
                Message = result.Message
            });
        }

    }
}