using Infrastructure.Services;
using Moq;
using StackExchange.Redis;

namespace Skinet.UnitTests.Infrastructure.Services;

[TestFixture]
public class ResponseChacheServiceTests
{
    private Mock<IConnectionMultiplexer> _redisMock = null!;
    private Mock<IDatabase> _databaseMock = null!;
    private ResponseChacheService _cacheService = null!;

    private Mock<IServer> _serverMock = null!;

    [SetUp]
    public void Setup()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _databaseMock = new Mock<IDatabase>();

        _redisMock
            .Setup(redis => redis.GetDatabase(1, null))
            .Returns(_databaseMock.Object);

        _cacheService = new ResponseChacheService(_redisMock.Object);

        _serverMock = new Mock<IServer>();

        var endpoint = new System.Net.IPEndPoint(
            System.Net.IPAddress.Loopback, 6379);

        _redisMock
            .Setup(redis => redis.GetEndPoints())
            .Returns([endpoint]);

        _redisMock
            .Setup(redis => redis.GetServer(endpoint, null))
            .Returns(_serverMock.Object);
    }


    [Test]
    public async Task ChachResponseAsync_StoresSerializedResponseWithExpiration()
    {
        // Arrange
        var cacheKey = "products";
        var response = new { Name = "Laptop", Price = 1000 };
        var timeToLive = TimeSpan.FromMinutes(5);

        // Act
        await _cacheService.ChachResponseAsync(
            cacheKey,
            response,
            timeToLive);

        // Assert
        var invocation = _databaseMock.Invocations.Single(
            invocation => invocation.Method.Name == nameof(IDatabase.StringSetAsync));

        Assert.Multiple(() =>
        {
            // Verify cache key
            Assert.That(
                invocation.Arguments[0]?.ToString(),
                Is.EqualTo(cacheKey));

            // Verify serialized JSON
            Assert.That(
                invocation.Arguments[1]?.ToString(),
                Is.EqualTo("""{"name":"Laptop","price":1000}"""));

            // Verify expiration
            Assert.That(
                invocation.Arguments[2]?.ToString(),
                Is.EqualTo("EX 300"));
        });
    }


    [Test]
    public async Task GetChachedRespopnseAsync_WhenKeyExists_ReturnsCachedResponse()
    {
        // Arrange
        var cacheKey = "products";
        var cachedResponse = """{"name":"Laptop","price":1000}""";

        _databaseMock
            .Setup(database => database.StringGetAsync(
                cacheKey,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)cachedResponse);

        // Act
        var result = await _cacheService.GetChachedRespopnseAsync(cacheKey);

        // Assert
        Assert.That(result, Is.EqualTo(cachedResponse));
    }


    [Test]
    public async Task RemoveCacheByPattern_WhenMatchingKeysExist_DeletesKeys()
    {
        // Arrange
        var pattern = "products:*";
        RedisKey[] keys =
        [
            "products:1",
        "products:2"
        ];

        _serverMock
            .Setup(server => server.Keys(
                1,
                pattern,
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<int>(),
                It.IsAny<CommandFlags>()))
            .Returns(keys);

        _databaseMock
            .Setup(database => database.KeyDeleteAsync(
                It.IsAny<RedisKey[]>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(keys.Length);

        // Act
        await _cacheService.RemoveCacheByPattern(pattern);

        // Assert
        var invocation = _databaseMock.Invocations.Single(
            invocation => invocation.Method.Name == nameof(IDatabase.KeyDeleteAsync));

        var deletedKeys = (RedisKey[])invocation.Arguments[0]!;

        Assert.Multiple(() =>
        {
            Assert.That(deletedKeys.Length, Is.EqualTo(2));
            Assert.That(deletedKeys.Select(key => key.ToString()),
                Is.EquivalentTo(new[] { "products:1", "products:2" }));
        });
    }

}
