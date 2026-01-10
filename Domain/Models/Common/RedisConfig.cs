namespace Domain.Models.Common
{
    public class RedisConfig
    {
        public required string ConnectionString { get; set; }
        public required string InstanceName { get; set; }
        public required string KeyPrefix { get; set; }
        public int MessageExpirationMinutes { get; set; } = 1440; // 24 giờ
        public int MaxMessageCount { get; set; } = 1000;
        public bool EnableCaching { get; set; } = true;
    }
} 