namespace Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiverEmail, string subject, string body);
    }
}
