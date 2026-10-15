using Core.Entities;
using Core.Interfases;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Skinet.UnitTests.Infrastructure.Services;


[TestFixture]
public class PaymentServiceTests
{
    private Mock<IConfiguration> _configMock = null!;
    private Mock<ICartService> _cartServiceMock = null!;
    private Mock<IUnitOfWork> _unitMock = null!;
    private PaymentService _paymentService = null!;


    [SetUp]
    public void Setup()
    {
        _configMock = new Mock<IConfiguration>();
        _cartServiceMock = new Mock<ICartService>();
        _unitMock = new Mock<IUnitOfWork>();

        _paymentService = new PaymentService(
            _configMock.Object,
            _cartServiceMock.Object,
            _unitMock.Object);
    }


    [Test]
    public async Task CreateOrUpdatePaymentIntent_WhenCartDoesNotExist_ReturnsNull()
    {
        // Arrange
        var cartId = "cart-1";

        _cartServiceMock
            .Setup(service => service.GetCartAsync(cartId))
            .ReturnsAsync((ShoppingCart?)null);

        // Act
        var result = await _paymentService.CreateOrUpdatePaymentIntent(cartId);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task CreateOrUpdatePaymentIntent_WhenDeliveryMethodDoesNotExist_ReturnsNull()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = "cart-1",
            DeliveryMethodId = 1
        };

        var repositoryMock = new Mock<IGenericRepository<DeliveryMethod>>();

        _cartServiceMock
            .Setup(service => service.GetCartAsync(cart.Id))
            .ReturnsAsync(cart);

        _unitMock
            .Setup(unit => unit.Repository<DeliveryMethod>())
            .Returns(repositoryMock.Object);

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync((DeliveryMethod?)null);

        // Act
        var result = await _paymentService.CreateOrUpdatePaymentIntent(cart.Id);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task CreateOrUpdatePaymentIntent_WhenProductDoesNotExist_ReturnsNull()
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
                ProductName = "Product 1",
                Price = 100,
                Quantity = 1,
                PictureUrl = "product.jpg",
                Brand = "Brand",
                Type = "Type"
            }
            ]
        };

        var repositoryMock = new Mock<IGenericRepository<Product>>();

        _cartServiceMock
            .Setup(service => service.GetCartAsync(cart.Id))
            .ReturnsAsync(cart);

        _unitMock
            .Setup(unit => unit.Repository<Product>())
            .Returns(repositoryMock.Object);

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _paymentService.CreateOrUpdatePaymentIntent(cart.Id);

        // Assert
        Assert.That(result, Is.Null);
    }


}
