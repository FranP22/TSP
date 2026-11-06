using StackExchange.Redis;
using System.Text.Json;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class RedisService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);

            await _db.StringSetAsync(key, json, expiration ?? TimeSpan.FromMinutes(10));
        }
    }
}
