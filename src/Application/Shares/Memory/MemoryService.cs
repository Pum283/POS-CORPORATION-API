using Microsoft.Extensions.Caching.Memory;

namespace Application;

[RegisterService(typeof(IMemoryService))]
public class MemoryService : IMemoryService
{
    private readonly IMemoryCache _cache;

    public MemoryService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            return value;
        }

        var result = await factory();
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = duration ?? TimeSpan.FromMinutes(30)
        };

        _cache.Set(key, result, options);
        return result;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}

