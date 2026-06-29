using Microsoft.Data.SqlClient;

namespace Infrastructure.Extensions.HangFire
{
    /// <summary>
    /// Đảm bảo database Hangfire tồn tại; nếu chưa có thì tạo (kết nối master rồi CREATE DATABASE).
    /// Có retry để chờ SQL Server sẵn sàng (Docker compose thường khởi động SQL chậm hơn app).
    /// </summary>
    public static class HangfireDatabaseEnsurer
    {
        private const int MaxRetries = 15;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);

        public static void EnsureHangfireDatabaseExists(string hangfireConnectionString)
        {
            if (string.IsNullOrWhiteSpace(hangfireConnectionString))
                return;

            for (var attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder(hangfireConnectionString);
                    var databaseName = builder.InitialCatalog;
                    if (string.IsNullOrWhiteSpace(databaseName))
                        return;

                    builder.InitialCatalog = "master";

                    using var conn = new SqlConnection(builder.ConnectionString);
                    conn.Open();

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"""
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'{databaseName.Replace("'", "''")}')
                        CREATE DATABASE [{databaseName.Replace("]", "]]")}]
                        """;
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (Exception)
                {
                    if (attempt == MaxRetries)
                        return;
                    Thread.Sleep(RetryDelay);
                }
            }
        }
    }
}
