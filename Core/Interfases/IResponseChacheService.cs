namespace Core.Interfases;

public interface IResponseChacheService
{
    Task ChachResponseAsync(string cachKey, object response, TimeSpan timeToLive);
    Task<string?> GetChachedRespopnseAsync(string cacheKey);
    Task RemoveCacheByPattern(string pattern);

}
