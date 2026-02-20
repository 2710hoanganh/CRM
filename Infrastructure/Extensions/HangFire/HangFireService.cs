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

        public Task EmailJob<T>(Expression<Func<T, Task>> job)
        {
            BackgroundJob.Enqueue(() => TestJob());
            return Task.CompletedTask;
        }

        public Task ReminderLoanRepayment3DaysJob()
        {
            // Khi có IEmailService: RecurringJob.AddOrUpdate<IEmailService>("...", x => x.SendMailReminderLoanRepayment3Days(), "0 0 * * *");
            RecurringJob.AddOrUpdate("reminder-loan-repayment-3-days", () => PlaceholderReminder3Days(), "0 0 * * *");
            return Task.CompletedTask;
        }

        public Task ReminderLoanRepayment1DayJob()
        {
            RecurringJob.AddOrUpdate("reminder-loan-repayment-1-day", () => PlaceholderReminder1Day(), "0 0 * * *");
            return Task.CompletedTask;
        }

        public Task ReminderLoanRepaymentLateHourJob()
        {
            RecurringJob.AddOrUpdate("reminder-loan-repayment-late-hour", () => PlaceholderReminderLateHour(), "0 13 * * *");
            return Task.CompletedTask;
        }

        public static void PlaceholderReminder3Days() => Console.WriteLine("[Recurring] Reminder 3 days placeholder");
        public static void PlaceholderReminder1Day() => Console.WriteLine("[Recurring] Reminder 1 day placeholder");
        public static void PlaceholderReminderLateHour() => Console.WriteLine("[Recurring] Reminder late 1 hour placeholder");

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