using Application.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string receiverEmail, string subject, string body)
        {
            _logger.LogInformation($"[Email Simulation] Sent to: {receiverEmail} | Subject: {subject} | Body: {body}");
            return Task.CompletedTask;
        }
    }
}
