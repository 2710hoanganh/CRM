using Application.Services;
using Domain.Models.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly VNPayConfig _config;

        public VNPayService(IOptions<VNPayConfig> config)
        {
            _config = config.Value;
        }

        public string CreatePaymentUrl(int userRepaymentId, decimal amount, string ipAddress)
        {
            return $"{_config.Url}?vnp_Amount={amount * 100}&vnp_Command=pay&vnp_CreateDate={DateTime.Now:yyyyMMddHHmmss}&vnp_CurrCode=VND&vnp_IpAddr={ipAddress}&vnp_OrderInfo=ThanhToanKhoanVay{userRepaymentId}&vnp_ReturnUrl={_config.ReturnUrl}&vnp_TmnCode={_config.TmnCode}&vnp_TxnRef={userRepaymentId}&vnp_Version=2.1.0&vnp_SecureHash=mockhash";
        }

        public bool ValidateSignature(IDictionary<string, string> queryParams, string vnp_SecureHash)
        {
            // Mock signature validation
            return true;
        }
    }
}
