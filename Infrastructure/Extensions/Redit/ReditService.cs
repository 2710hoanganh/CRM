using Application.Services;
using StackExchange.Redis;
using System.Text.Json;
using System.Linq;

namespace Infrastructure.Extensions.Redit
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default!;

            return JsonSerializer.Deserialize<T>(value!)!;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(value), expiry == null ? TimeSpan.Zero : expiry.Value);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}