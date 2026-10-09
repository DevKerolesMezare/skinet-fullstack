
using Core.Entities;
using Core.Entities.OrderAggregate;
using NUnit.Framework;

namespace Skinet.UnitTests.Core.Entities.OrderAggregate;

[TestFixture]
public class OrderTests
{
    [Test]
    public void GetTotal_ShouldReturnSubtotalPlusDeliveryPrice()
    {
        var order = new Order
        {
            BuyerEmail = "test@example.com",
            ShippingAddress = null!,
            PaymentIntentId = "pi_test",
            Subtotal = 100m,
            DeliveryMethod = new DeliveryMethod
            {
                ShortName = "STD",
                DeliveryTime = "3-5 Days",
                Description = "Standard delivery",
                Price = 20m
            }
        };

        var result = order.GetTotal();

        Assert.That(result, Is.EqualTo(120m));
    }
}
