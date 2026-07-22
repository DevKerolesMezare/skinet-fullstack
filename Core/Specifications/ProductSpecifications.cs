using Core.Entities;

namespace Core.Specifications;

public class ProductSpecifications : BaseSpecification<Product>
{
    public ProductSpecifications(ProductSpecParams specParams)
        : base(x =>
            (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search)) &&
            (!specParams.Brands.Any() || specParams.Brands.Contains(x.Brand)) &&
            (!specParams.Types.Any() || specParams.Types.Contains(x.Type))
        )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        switch (specParams.Sort)
        {
            case "PriceAsc":
                AddOrderBy(x => x.Price);
                break;

            case "PriceDesc":
                AddOrderByDescending(x => x.Price);
                break;

            default:
                AddOrderBy(x => x.Name);
                break;
        }
    }

}
