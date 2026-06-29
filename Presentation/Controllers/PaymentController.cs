using Application.Features.Loan.Command;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;
        private readonly IMediator _mediator;

        public PaymentController(IVNPayService vnPayService, IMediator mediator)
        {
            _vnPayService = vnPayService;
            _mediator = mediator;
        }

        [HttpPost("create-url")]
        public IActionResult CreatePaymentUrl([FromQuery] int userRepaymentId, [FromQuery] decimal amount)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var url = _vnPayService.CreatePaymentUrl(userRepaymentId, amount, ipAddress);
            return Ok(new { Url = url });
        }

        [HttpGet("vnpay-return")]
        public IActionResult VNPayReturn([FromQuery] Dictionary<string, string> queryParams)
        {
            // Just display to user
            return Ok(new { Message = "Payment processed. Please check your repayment status." });
        }

        [HttpPost("vnpay-ipn")]
        public async Task<IActionResult> VNPayIPN([FromQuery] Dictionary<string, string> queryParams)
        {
            // 1. Validate signature
            string secureHash = queryParams.GetValueOrDefault("vnp_SecureHash") ?? "";
            if (!_vnPayService.ValidateSignature(queryParams, secureHash))
            {
                return BadRequest("Invalid signature");
            }

            // 2. Extract Data
            if (!int.TryParse(queryParams.GetValueOrDefault("vnp_TxnRef"), out var userRepaymentId))
            {
                return BadRequest("Invalid TxnRef");
            }

            if (!decimal.TryParse(queryParams.GetValueOrDefault("vnp_Amount"), out var amountValue))
            {
                return BadRequest("Invalid Amount");
            }

            decimal actualAmount = amountValue / 100;

            // 3. Dispatch command to pay loan
            var command = new PayLoanCommand
            {
                UserRepaymentId = userRepaymentId,
                AmountToPay = actualAmount,
                ReferenceNumber = queryParams.GetValueOrDefault("vnp_TransactionNo")
            };

            try
            {
                // Note: Race condition handling (Redis lock) should be placed here in a real scenario
                var result = await _mediator.Send(command);
                if (result)
                {
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                return BadRequest(new { RspCode = "99", Message = "Failed to process" });
            }
            catch (Exception ex)
            {
                // In IPN, usually return a specific code for error
                return BadRequest(new { RspCode = "99", Message = ex.Message });
            }
        }
    }
}
