using System.Text.Json;
using Core.Interfases;
using StackExchange.Redis;
namespace Infrastructure.Services;

public class ResponseChacheService(IConnectionMultiplexer redis) : IResponseChacheService
{
    private readonly IDatabase _database = redis.GetDatabase(1);

    public async Task ChachResponseAsync(string cachKey, object response, TimeSpan timeToLive)
    {
        var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var serializedResponse = JsonSerializer.Serialize(response, option);

        await _database.StringSetAsync(cachKey, serializedResponse, timeToLive);

    }

    public async Task<string?> GetChachedRespopnseAsync(string cacheKey)
    {
        var chachedResponse = await _database.StringGetAsync(cacheKey);

        if (chachedResponse.IsNullOrEmpty) return null;

        return chachedResponse;
    }

    public async Task RemoveCacheByPattern(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(database: 1, pattern: $"{pattern}").ToArray();

        if (keys.Length != 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
