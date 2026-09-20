using Core.Entities;

namespace Core.Interfases;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
    Task<string> RefundPayment(string paymentIntentId);
}
