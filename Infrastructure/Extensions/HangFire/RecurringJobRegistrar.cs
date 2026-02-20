using Application.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.HangFire
{
    /// <summary>
    /// Đăng ký tất cả recurring job (Clean Architecture: implementation nằm ở Infrastructure, dùng Hangfire).
    /// </summary>
    public class RecurringJobRegistrar : IRecurringJobRegistrar
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RecurringJobRegistrar(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void RegisterRecurringJobs()
        {
            RecurringJob.AddOrUpdate("test-hourly", () => HangFireService.RunHourlyJob(), "0 * * * *");

            using var scope = _scopeFactory.CreateScope();
            var hangFire = scope.ServiceProvider.GetRequiredService<IHangFireService>();
            hangFire.ReminderLoanRepayment3DaysJob();
            hangFire.ReminderLoanRepayment1DayJob();
            hangFire.ReminderLoanRepaymentLateHourJob();
        }
    }
}
