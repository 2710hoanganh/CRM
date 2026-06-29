namespace Application.Services
{
    /// <summary>
    /// Đăng ký các recurring job khi ứng dụng khởi động (Clean Architecture: contract thuộc Application).
    /// Implementation chứa logic Hangfire nằm ở Infrastructure.
    /// </summary>
    public interface IRecurringJobRegistrar
    {
        void RegisterRecurringJobs();
    }
}
