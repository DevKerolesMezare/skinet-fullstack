using Core.Entities;
using Infrastructure.Services;
using Moq;
using StackExchange.Redis;

namespace Skinet.UnitTests;

[TestFixture]
public class CartServiceTests
{
    private Mock<IDatabase> _databaseMock = null!;
    private Mock<IConnectionMultiplexer> _redisMock = null!;
    private CartService _cartService = null!;

    [SetUp]
    public void Setup()
    {
        _databaseMock = new Mock<IDatabase>();
        _redisMock = new Mock<IConnectionMultiplexer>();

        _redisMock
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_databaseMock.Object);

        _cartService = new CartService(_redisMock.Object);
    }


    [Test]
    public async Task GetCartAsync_WhenCartDoesNotExist_ReturnsNull()
    {
        // Arrange
        _databaseMock
            .Setup(db => db.StringGetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cartService.GetCartAsync("cart-1");

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task GetCartAsync_WhenCartExists_ReturnsCart()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1",
            Items =
            [
                new CarItem
            {
                ProductId = 1,
                ProductName = "Bike",
                Price = 100,
                Quantity = 2,
                PictureUrl = "bike.jpg",
                Brand = "Brand A",
                Type = "Electric"
            }
            ]
        };

        var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);

        _databaseMock
            .Setup(db => db.StringGetAsync(
                "cart-1",
                It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)cartJson);

        // Act
        var result = await _cartService.GetCartAsync("cart-1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(cart.Id));
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].ProductName, Is.EqualTo("Bike"));
        Assert.That(result.Items[0].Quantity, Is.EqualTo(2));
    }


    [Test]
    public async Task SetCartAsync_WhenSaveSucceeds_ReturnsCart()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1"
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _databaseMock
            .Setup(db => db.StringGetAsync(
                cart.Id,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(
                (RedisValue)System.Text.Json.JsonSerializer.Serialize(cart));

        // Act
        var result = await _cartService.SetCartAsync(cart);

        // Assert
        _databaseMock.Verify(
            db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()),
            Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(cart.Id));


        _redisMock.Verify(
        r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()),
        Times.Once);
    }


    [Test]
    public async Task SetCartAsync_WhenSaveFails_ReturnsNull()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1"
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        var result = await _cartService.SetCartAsync(cart);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task DeleteCartAsync_WhenCartExists_ReturnsTrue()
    {
        // Arrange
        var cartId = "cart-1";

        _databaseMock
            .Setup(db => db.KeyDeleteAsync(
                cartId,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await _cartService.DeleteCartAsync(cartId);

        // Assert
        Assert.That(result, Is.True);
    }


    [Test]
    public async Task DeleteCartAsync_WhenCartDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cartId = "cart-1";

        _databaseMock
            .Setup(db => db.KeyDeleteAsync(
                cartId,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        var result = await _cartService.DeleteCartAsync(cartId);

        // Assert
        Assert.That(result, Is.False);
    }



    [Test]
    public async Task GetCartAsync_WhenCartDataIsEmpty_ReturnsNull()
    {
        // Arrange
        var cartId = "cart-1";

        _databaseMock
            .Setup(db => db.StringGetAsync(
                cartId,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.EmptyString);

        // Act
        var result = await _cartService.GetCartAsync(cartId);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task DeleteCartAsync_CallsRedisOnce()
    {
        // Arrange
        var cartId = "cart-1";

        _databaseMock
            .Setup(db => db.KeyDeleteAsync(
                cartId,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _cartService.DeleteCartAsync(cartId);

        // Assert
        _databaseMock.Verify(
            db => db.KeyDeleteAsync(
                cartId,
                It.IsAny<CommandFlags>()),
            Times.Once);
    }


    [Test]
    public async Task SetCartAsync_SetsExpirationTo30Days()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1"
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _databaseMock
            .Setup(db => db.StringGetAsync(
                cart.Id,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(
                (RedisValue)System.Text.Json.JsonSerializer.Serialize(cart));

        // Act
        await _cartService.SetCartAsync(cart);

        // Assert

        _databaseMock.Verify(
            db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.Is<Expiration>(expiry =>
                    expiry.Equals((Expiration)TimeSpan.FromDays(30))),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()),
            Times.Once);

    }



    [Test]
    public async Task SetCartAsync_WhenSaveFails_DoesNotRetrieveCart()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1"
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                cart.Id,
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        await _cartService.SetCartAsync(cart);

        // Assert
        _databaseMock.Verify(
            db => db.StringGetAsync(
                cart.Id,
                It.IsAny<CommandFlags>()),
            Times.Never);
    }


    [Test]
    public async Task GetCartAsync_CallsRedisOnce()
    {
        // Arrange
        var cartId = "cart-1";

        _databaseMock
            .Setup(db => db.StringGetAsync(
                cartId,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        await _cartService.GetCartAsync(cartId);

        // Assert
        _databaseMock.Verify(
            db => db.StringGetAsync(
                cartId,
                It.IsAny<CommandFlags>()),
            Times.Once);
    }


    [Test]
    public async Task GetCartAsync_WhenCartExists_ReturnsCartItems()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1",
            Items =
            [
                new CarItem
            {
                ProductId = 1,
                ProductName = "Laptop",
                Price = 25000m,
                Quantity = 2,
                PictureUrl = "laptop.jpg",
                Brand = "Dell",
                Type = "Electronics"
            }
            ]
        };

        var json = System.Text.Json.JsonSerializer.Serialize(cart);

        _databaseMock
            .Setup(db => db.StringGetAsync(
                cart.Id,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)json);

        // Act
        var result = await _cartService.GetCartAsync(cart.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].ProductId, Is.EqualTo(1));
        Assert.That(result.Items[0].Price, Is.EqualTo(25000m));
        Assert.That(result.Items[0].Quantity, Is.EqualTo(2));
    }


}
