using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.Notification;
using Application.Features.Notification.Query;
using Presentation.DTOs;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notification")]
    public class NotifcationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotifcationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListNotification([FromQuery] BasePaginationQueryDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListNoitifiactionQuery
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);

            return Ok(new Response<List<NotficationList>>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Message
            });
        }
    }
}