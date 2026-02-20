using Application.Services;
using System.Linq.Expressions;
using Hangfire;

namespace Infrastructure.Extensions.HangFire
{
    public class HangFireService : IHangFireService
    {
        public Task ScheduleJob<T>(Expression<Func<T, Task>> job, TimeSpan delayTime)
        {
            BackgroundJob.Schedule(() => TestJob(), delayTime);
            return Task.CompletedTask;
        }

        /// <summary>Đăng ký job chạy lặp theo cron. Cron: phút giờ ngày tháng thứ.</summary>
        public void AddRecurringJob(string jobId, Action job, string cronExpression)
        {
            RecurringJob.AddOrUpdate(jobId, () => TestJob(), cronExpression);
        }

        /// <summary>Public để Hangfire worker có thể gọi khi xử lý job từ queue.</summary>
        public void TestJob()
        {
            Console.WriteLine("Test Job");
        }

        /// <summary>Job chạy hàng giờ (static để RecurringJob đăng ký không cần resolve service).</summary>
        public static void RunHourlyJob()
        {
            Console.WriteLine($"[Recurring] Hourly job at {DateTime.UtcNow:O}");
        }
    }
}