namespace Application;

public interface IMemoryService
{
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null);
    void Remove(string key);
}
