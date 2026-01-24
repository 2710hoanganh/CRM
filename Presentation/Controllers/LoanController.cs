using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Features.Loan.Query;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.Loan;
using Application.Features.Loan.Command;
using Presentation.DTOs;
using Domain.Models.DTO.UserRepayment;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/loan")]
    public class LoanController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // [Authorize(Roles = "0,1")]
        [HttpGet("all-admin")]
        public async Task<IActionResult> GetAllLoan([FromQuery] BasePaginationQueryDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllLoanQuery
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);
            return Ok(new Response<Paged<List<ListLoanResponse>>>(ResponseResult.SUCCESS)
            {
                Data = result,
                Message = result.Message
            });
        }

        // [Authorize(Roles = "2")]
        [HttpGet("all-user")]
        public async Task<IActionResult> GetAllUserLoan([FromQuery] BasePaginationQueryDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllUserLoanQuery
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);
            return Ok(new Response<Paged<List<ListLoanResponse>>>(ResponseResult.SUCCESS)
            {
                Data = result,
                Message = result.Message
            });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetLoanInfo([FromQuery] BaseFieldsDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLoanInfoQuery
            {
                Id = query.Id,
            }, cancellationToken);

            return Ok(new Response<GetLoanInfoResponse>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Loan info found" : "Loan info not found"
            });
        }

        [HttpGet("repayment")]
        public async Task<IActionResult> GetLoanRepaymentDate([FromQuery] BaseFieldsDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLoanRepaymentDateQuery
            {
                Id = query.Id,
            }, cancellationToken);
                
            return Ok(new Response<UserListRepayment>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Loan repayment dates retrieved successfully" : "Loan repayment dates not found"
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanCommand command, CancellationToken cancellationToken)
        {
            // get user id from token
            command.Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new Response<bool>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Loan created successfully" : "Loan created failed"
            });
        }

        // [Authorize(Roles = "0,1")]
        [HttpPost("review")]
        public async Task<IActionResult> ReviewLoan([FromBody] ReviewLoanCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new Response<bool>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Loan reviewed successfully" : "Loan reviewed failed"
            });
        }
    }
}