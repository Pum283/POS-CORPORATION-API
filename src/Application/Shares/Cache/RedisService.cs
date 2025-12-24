using Contract;
using StackExchange.Redis;

namespace Application;

public class RedisService(IConnectionMultiplexer redisConnection) : IRedisService
{
    private readonly IConnectionMultiplexer _redis = redisConnection;

    private IDatabase GetDatabase() => _redis.GetDatabase();

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonHelper.Serialize(value);
        return await GetDatabase().StringSetAsync(key, json, expiry).ConfigureAwait(false);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await GetDatabase().StringGetAsync(key).ConfigureAwait(false);
        return value.HasValue ? JsonHelper.Deserialize<T>(value!) : default;
    }

    public async Task<bool> DeleteAsync(string key) => await GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);

    public async Task<bool> ExistsAsync(string key) => await GetDatabase().KeyExistsAsync(key).ConfigureAwait(false);

    public async Task<long> IncrementAsync(string key, long value = 1) => await GetDatabase().StringIncrementAsync(key, value).ConfigureAwait(false);

    public async Task<long> DecrementAsync(string key, long value = 1) => await GetDatabase().StringDecrementAsync(key, value).ConfigureAwait(false);

    public async Task SetExpiryAsync(string key, TimeSpan expiry) => await GetDatabase().KeyExpireAsync(key, expiry).ConfigureAwait(false);

    public async Task SetHashFieldAsync(string hashKey, string field, string value) => await GetDatabase().HashSetAsync(hashKey, field, value).ConfigureAwait(false);

    public async Task<string?> GetHashFieldAsync(string hashKey, string field) => await GetDatabase().HashGetAsync(hashKey, field).ConfigureAwait(false);

    public async Task<Dictionary<string, string>> GetAllHashFieldsAsync(string hashKey)
    {
        var entries = await GetDatabase().HashGetAllAsync(hashKey).ConfigureAwait(false);
        return entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
    }

    public async Task PushLeftAsync(string key, string value) => await GetDatabase().ListLeftPushAsync(key, value).ConfigureAwait(false);

    public async Task PushRightAsync(string key, string value) => await GetDatabase().ListRightPushAsync(key, value).ConfigureAwait(false);

    public async Task<string?> PopLeftAsync(string key) => await GetDatabase().ListLeftPopAsync(key).ConfigureAwait(false);

    public async Task<string?> PopRightAsync(string key) => await GetDatabase().ListRightPopAsync(key).ConfigureAwait(false);

    public async Task AddToSetAsync(string key, string value) => await GetDatabase().SetAddAsync(key, value).ConfigureAwait(false);

    public async Task<bool> IsMemberOfSetAsync(string key, string value) => await GetDatabase().SetContainsAsync(key, value).ConfigureAwait(false);

    public async Task AddToSortedSetAsync(string key, string value, double score) => await GetDatabase().SortedSetAddAsync(key, value, score).ConfigureAwait(false);

    public async Task<bool> RemoveFromSortedSetAsync(string key, string value) => await GetDatabase().SortedSetRemoveAsync(key, value).ConfigureAwait(false);

    public async Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop) => await GetDatabase().SortedSetRangeByRankWithScoresAsync(key, start, stop).ConfigureAwait(false);

    public Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).Select(k => k.ToString());
        return Task.FromResult(keys);
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null && !EqualityComparer<T>.Default.Equals(cached, default!))
        {
            return cached;
        }

        // Nếu không có thì gọi factory để tạo mới
        var value = await factory();
        if (value is not null)
        {
            await SetAsync(key, value, expiry);
        }

        return value;
    }
}
