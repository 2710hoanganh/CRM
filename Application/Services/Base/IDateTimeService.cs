namespace Application.Services.Base
{
    public interface IDateTimeService
    {
        Task<DateTime> GetRepaymentDate(DateTime startDate, int monthIndex, CancellationToken cancellationToken = default);
    }
}