using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfases;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO orderDTO)
    {
        var email = User.GetEmail();

        var cart = await cartService.GetCartAsync(orderDTO.CartId);

        if (cart == null) return BadRequest("Cart not found");

        // Continue with order creation logic

        if (cart.PaymentIntenId == null)
        {
            return BadRequest("No payment intent found for the cart");
        }

        var itmes = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest($"Product with id {item.ProductId} not found");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl
            };

            var orderItem = new OrderItem
            {

                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };

            itmes.Add(orderItem);
        }

        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDTO.DeliveryMethodId);

        if (deliveryMethod == null) return BadRequest($"Delivery method with id {orderDTO.DeliveryMethodId} not found");

        var order = new Order
        {
            OrderItems = itmes,
            DeliveryMethod = deliveryMethod,
            BuyerEmail = email,
            ShippingAddress = orderDTO.ShippingAddress,
            Subtotal = itmes.Sum(item => item.Price * item.Quantity),
            PaymentSummary = orderDTO.PaymentSummary,
            PaymentIntentId = cart.PaymentIntenId
        };

        unit.Repository<Order>().Add(order);

        if (await unit.Complete())
            return order;

        return BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrdersForUser()
    {
        var specification = new OrderSpecification(User.GetEmail());

        var orders = await unit.Repository<Order>().ListAsync(specification);

        var ordersToReturn = orders.Select(order => order.ToDto()).ToList();

        return Ok(ordersToReturn);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
    {
        var specification = new OrderSpecification(id, User.GetEmail());

        var order = await unit.Repository<Order>().GetEntityWithSpec(specification);

        if (order == null) return NotFound();

        return order.ToDto();
    }
}
