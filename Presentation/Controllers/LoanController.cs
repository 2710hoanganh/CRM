using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Features.Loan.Query;
using Domain.Models.Common;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.Loan;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetLoanInfo()
        {
            var result = await _mediator.Send(new GetLoanInfoQuery
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0")
            });

            return Ok(new Response<GetLoanInfoResponse>(ResponseResult.SUCCESS)
            {
                Data = result.Data,
                Message = result.Result == ResponseResult.SUCCESS ? "Loan info found" : "Loan info not found"
            });
        }
    }
}