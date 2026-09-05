using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions;

public static class OrderMappingExtensions
{
    public static OrderDTO ToDto(this Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            BuyerEmail = order.BuyerEmail,
            ShippingAddress = order.ShippingAddress,
            DeliveryMethod = order.DeliveryMethod.ShortName,
            PaymentSummary = order.PaymentSummary,
            ShippingPrice = order.DeliveryMethod.Price,
            OrderItems = order.OrderItems.Select(item => item.ToDto()).ToList(),
            Subtotal = order.Subtotal,
            Total = order.GetTotal(),
            Status = order.Status.ToString(),
            PaymentIntentId = order.PaymentIntentId
        };
    }

    public static OrderItemDTO ToDto(this OrderItem orderItem)
    {
        return new OrderItemDTO
        {
            ProductId = orderItem.ItemOrdered.ProductId,
            ProductName = orderItem.ItemOrdered.ProductName,
            PictureUrl = orderItem.ItemOrdered.PictureUrl,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        };
    }

}
