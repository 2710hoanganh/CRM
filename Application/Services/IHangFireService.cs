using System.Linq.Expressions;

namespace Application.Services
{
    public interface IHangFireService
    {
        Task ScheduleJob<T>(Expression<Func<T, Task>> job, TimeSpan delay);
        Task EmailJob<T>(Expression<Func<T, Task>> job);
        /// <summary>Đăng ký recurring job nhắc trả nợ trước 3 ngày (chạy 00:00 mỗi ngày).</summary>
        Task ReminderLoanRepayment3DaysJob();
        /// <summary>Đăng ký recurring job nhắc trả nợ trước 1 ngày (chạy 00:00 mỗi ngày).</summary>
        Task ReminderLoanRepayment1DayJob();
        /// <summary>Đăng ký recurring job nhắc trả nợ trễ 1 giờ (chạy 13:00 mỗi ngày).</summary>
        Task ReminderLoanRepaymentLateHourJob();
        void AddRecurringJob(string jobId, Action job, string cronExpression);
    }
}