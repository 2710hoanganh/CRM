using Application.Services.Base;

namespace Infrastructure.Services
{
    public class DateTimeService: IDateTimeService
    {
        public Task<DateTime> GetRepaymentDate(DateTime startDate, int monthIndex, CancellationToken cancellationToken = default)
        {

            var month = startDate.AddMonths(monthIndex);
            var dayInMonth = DateTime.DaysInMonth(month.Year, month.Month);
            var day = Math.Min(startDate.Day, dayInMonth);
            var repaymentDate = new DateTime(month.Year, month.Month, day);
            return Task.FromResult(repaymentDate);
        }
    }
}