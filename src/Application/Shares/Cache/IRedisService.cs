using StackExchange.Redis;

namespace Application;

public interface IRedisService
{
    /// <summary>
    /// Stores a value in Redis as JSON with a specific key.
    /// </summary>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// Retrieves a value from Redis and deserializes it into a specific type.
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Deletes a key from Redis.
    /// </summary>
    Task<bool> DeleteAsync(string key);

    /// <summary>
    /// Checks whether a key exists in Redis.
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Increments the value of a key by a specified amount (applies to numeric values).
    /// </summary>
    Task<long> IncrementAsync(string key, long value = 1);

    /// <summary>
    /// Decrements the value of a key by a specified amount (applies to numeric values).
    /// </summary>
    Task<long> DecrementAsync(string key, long value = 1);

    /// <summary>
    /// Sets an expiration time for a key in Redis.
    /// </summary>
    Task SetExpiryAsync(string key, TimeSpan expiry);

    /// <summary>
    /// Sets a specific field in a hash.
    /// </summary>
    Task SetHashFieldAsync(string hashKey, string field, string value);

    /// <summary>
    /// Retrieves the value of a specific field from a hash.
    /// </summary>
    Task<string?> GetHashFieldAsync(string hashKey, string field);

    /// <summary>
    /// Retrieves all fields and values from a hash.
    /// </summary>
    Task<Dictionary<string, string>> GetAllHashFieldsAsync(string hashKey);

    /// <summary>
    /// Adds a value to a list (FIFO - pushes to the front).
    /// </summary>
    Task PushLeftAsync(string key, string value);

    /// <summary>
    /// Adds a value to a list (LIFO - pushes to the back).
    /// </summary>
    Task PushRightAsync(string key, string value);

    /// <summary>
    /// Removes and retrieves the first value from a list.
    /// </summary>
    Task<string?> PopLeftAsync(string key);

    /// <summary>
    /// Removes and retrieves the last value from a list.
    /// </summary>
    Task<string?> PopRightAsync(string key);

    /// <summary>
    /// Adds an element to a set.
    /// </summary>
    Task AddToSetAsync(string key, string value);

    /// <summary>
    /// Checks if an element exists in a set.
    /// </summary>
    Task<bool> IsMemberOfSetAsync(string key, string value);

    /// <summary>
    /// Adds an element to a sorted set with a specific score.
    /// </summary>
    Task AddToSortedSetAsync(string key, string value, double score);

    /// <summary>
    /// Removes an element from a sorted set.
    /// </summary>
    Task<bool> RemoveFromSortedSetAsync(string key, string value);

    /// <summary>
    /// Retrieves elements from a sorted set based on ranking.
    /// </summary>
    Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop);

    /// <summary>
    /// Retrieves a list of keys that match a pattern.
    /// </summary>
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
}