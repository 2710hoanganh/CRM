using Application.Services;
using Hangfire;

namespace Infrastructure.Extensions.HangFire
{
    /// <summary>
    /// Đăng ký tất cả recurring job (Clean Architecture: implementation nằm ở Infrastructure, dùng Hangfire).
    /// </summary>
    public class RecurringJobRegistrar : IRecurringJobRegistrar
    {
        public void RegisterRecurringJobs()
        {
            // Job chạy hàng giờ (phút 0 của mỗi giờ)
            RecurringJob.AddOrUpdate("test-hourly", () => HangFireService.RunHourlyJob(), "0 * * * *");
        }
    }
}
