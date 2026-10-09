using Core.Entities.OrderAggregate;
using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class OrderSpecificationTests
{
    [Test]
    public void Constructor_WithEmail_ShouldSetIncludesAndOrderByDescending()
    {
        // Arrange
        var spec = new OrderSpecification("test@example.com");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(spec.Criteria, Is.Not.Null);
            Assert.That(spec.Includes, Has.Count.EqualTo(2));
            Assert.That(spec.OrderByDescending, Is.Not.Null);
        });
    }

    [Test]
    public void Constructor_WithIdAndEmail_ShouldSetCriteriaAndIncludes()
    {
        // Arrange
        var spec = new OrderSpecification(10, "test@example.com");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(spec.Criteria, Is.Not.Null);
            Assert.That(spec.IncludeStrings, Has.Count.EqualTo(2));
        });
    }


    [Test]
    public void Constructor_WithSpecParams_ShouldApplyPagingAndSorting()
    {
        // Arrange
        var specParams = new OrderSpecParams
        {
            PageIndex = 2,
            PageSize = 5,
            Status = ""
        };

        // Act
        var spec = new OrderSpecification(specParams);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(spec.IsPagingEnabled, Is.True);
            Assert.That(spec.Skip, Is.EqualTo(5));
            Assert.That(spec.Take, Is.EqualTo(5));
            Assert.That(spec.OrderByDescending, Is.Not.Null);
            Assert.That(spec.IncludeStrings, Has.Count.EqualTo(2));
        });
    }



    [Test]
    public void Constructor_WithPaymentIntentId_ShouldSetCriteriaAndIncludes()
    {
        // Arrange
        var spec = new OrderSpecification("pi_test123", true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(spec.Criteria, Is.Not.Null);
            Assert.That(spec.IncludeStrings, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Constructor_WithId_ShouldSetCriteriaAndIncludes()
    {
        // Arrange
        var spec = new OrderSpecification(10);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(spec.Criteria, Is.Not.Null);
            Assert.That(spec.IncludeStrings, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Constructor_WithId_ShouldFilterOrdersById()
    {
        // Arrange
        var orders = new List<Order>
    {
        new()
        {
            Id = 1,
            BuyerEmail = "a@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_1"
        },
        new()
        {
            Id = 2,
            BuyerEmail = "b@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_2"
        }
    };

        var spec = new OrderSpecification(1);

        // Act
        var result = spec.ApplayCriteria(orders.AsQueryable()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(1));
    }

    [Test]
    public void Constructor_WithIdAndEmail_ShouldFilterByBothConditions()
    {
        // Arrange
        var orders = new List<Order>
    {
        new()
        {
            Id = 1,
            BuyerEmail = "a@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_1"
        },
        new()
        {
            Id = 1,
            BuyerEmail = "b@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_2"
        }
    };

        var spec = new OrderSpecification(1, "a@test.com");

        // Act
        var result = spec.ApplayCriteria(orders.AsQueryable()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].BuyerEmail, Is.EqualTo("a@test.com"));
    }

    [Test]
    public void Constructor_WithStatus_ShouldFilterOrdersByStatus()
    {
        // Arrange
        var orders = new List<Order>
    {
        new()
        {
            Id = 1,
            BuyerEmail = "a@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_1",
            Status = OrderStatus.Pending
        },
        new()
        {
            Id = 2,
            BuyerEmail = "b@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_2",
            Status = OrderStatus.PaymentReceived
        }
    };

        var specParams = new OrderSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Status = "Pending"
        };

        var spec = new OrderSpecification(specParams);

        // Act
        var result = spec.ApplayCriteria(orders.AsQueryable()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Status, Is.EqualTo(OrderStatus.Pending));
    }

    [Test]
    public void Constructor_WithEmptyStatus_ShouldReturnAllOrders()
    {
        var orders = new List<Order>
    {
        new()
        {
            Id = 1,
            BuyerEmail = "a@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_1",
            Status = OrderStatus.Pending
        },
        new()
        {
            Id = 2,
            BuyerEmail = "b@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_2",
            Status = OrderStatus.PaymentReceived
        }
    };

        var specParams = new OrderSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Status = ""
        };

        var spec = new OrderSpecification(specParams);
        var result = spec.ApplayCriteria(orders.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void Constructor_WithInvalidStatus_ShouldReturnNoOrders()
    {
        var orders = new List<Order>
    {
        new()
        {
            Id = 1,
            BuyerEmail = "a@test.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_1",
            Status = OrderStatus.Pending
        }
    };

        var specParams = new OrderSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Status = "InvalidStatus"
        };

        var spec = new OrderSpecification(specParams);
        var result = spec.ApplayCriteria(orders.AsQueryable()).ToList();

        Assert.That(result, Is.Empty);
    }
}