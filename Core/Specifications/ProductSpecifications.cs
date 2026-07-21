using Core.Entities;

namespace Core.Specifications;

public class ProductSpecifications : BaseSpecification<Product>
{
    public ProductSpecifications(string? brand, string? type , string?sort) : base(x =>
    (string.IsNullOrWhiteSpace(brand) || x.Brand == brand) &&
    (string.IsNullOrWhiteSpace(type) || x.Type == type))
    {
        switch (sort)
        {
            case "PriceAsc":
                AddOrderBy(x => x.Price);
                break;

            case "PriceDesc":
                AddOrderByDescending(x =>x.Price);
                break;

            default:
                AddOrderBy(x => x.Name);
                break;
        }
    }

}
