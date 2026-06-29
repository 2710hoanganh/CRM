namespace Application.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(int userRepaymentId, decimal amount, string ipAddress);
        bool ValidateSignature(IDictionary<string, string> queryParams, string vnp_SecureHash);
    }
}
