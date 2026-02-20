using Microsoft.Data.SqlClient;

namespace Infrastructure.Extensions.HangFire
{
    /// <summary>
    /// Đảm bảo database Hangfire tồn tại; nếu chưa có thì tạo (kết nối master rồi CREATE DATABASE).
    /// </summary>
    public static class HangfireDatabaseEnsurer
    {
        public static void EnsureHangfireDatabaseExists(string hangfireConnectionString)
        {
            if (string.IsNullOrWhiteSpace(hangfireConnectionString))
                return;

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
            }
            catch (Exception)
            {
                // Không throw để app vẫn chạy; Hangfire sẽ lỗi khi dùng nếu DB chưa có
                // Có thể log ở đây nếu cần
            }
        }
    }
}
