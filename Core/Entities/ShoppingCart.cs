namespace Core.Entities;

public class ShoppingCart
{
    public required string   Id { get; set; }
    public List<CarItem> Items { get; set; } = [];
}
