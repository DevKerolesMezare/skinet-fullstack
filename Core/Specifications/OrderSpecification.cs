using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.DeliveryMethod);
        AddInclude(x => x.OrderItems);
        AddOrderByDescending(x => x.OrderDate);
    }

    public OrderSpecification(int id, string email) : base(x => x.Id == id && x.BuyerEmail == email)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }
}  
