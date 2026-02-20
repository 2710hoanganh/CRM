using System.Linq.Expressions;

namespace Application.Services
{
    public interface IHangFireService
    {
        Task ScheduleJob<T>(Expression<Func<T, Task>> job, TimeSpan delay);
        /// <summary>Đăng ký job chạy lặp theo cron (vd: hàng giờ).</summary>
        void AddRecurringJob(string jobId, Action job, string cronExpression);
    }
}