using Core.Entities;
using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class ProductSpecificationsTests
{
    [Test]
    public void Constructor_WithSearch_ShouldFilterProductsByName()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Nike Shoes", Brand = "Nike", Type = "Shoes", Description = "Comfortable running shoes", PictureUrl = "nike-shoes.png" },
            new() { Id = 2, Name = "Adidas Shirt", Brand = "Adidas", Type = "Clothing", Description = "Cotton athletic shirt", PictureUrl = "adidas-shirt.png" }
        };

        var specParams = new ProductSpecParams
        {
            Search = "Nike",
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public void Constructor_WithBrand_ShouldFilterProductsByBrand()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Adidas Shoes",
            Brand = "Adidas",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "adidas.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Brands = ["Nike"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Brand, Is.EqualTo("Nike"));
    }

    [Test]
    public void Constructor_WithType_ShouldFilterProductsByType()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Nike Shirt",
            Brand = "Nike",
            Type = "Clothing",
            Description = "Athletic shirt",
            PictureUrl = "shirt.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Types = ["Shoes"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Type, Is.EqualTo("Shoes"));
    }

    [Test]
    public void Constructor_WithLowercaseSearch_ShouldMatchProductName()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Search = "nike",
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public void Constructor_WithUnmatchedSearch_ShouldReturnEmptyList()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Search = "Adidas",
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Constructor_WithMultipleBrands_ShouldFilterMatchingProducts()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Adidas Shoes",
            Brand = "Adidas",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "adidas.png"
        },
        new()
        {
            Id = 3,
            Name = "Puma Shoes",
            Brand = "Puma",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "puma.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Brands = ["Nike", "Adidas"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(x => x.Brand), Is.EquivalentTo(["Nike", "Adidas"]));
    }

    [Test]
    public void Constructor_WithMultipleTypes_ShouldFilterMatchingProducts()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Nike Shirt",
            Brand = "Nike",
            Type = "Clothing",
            Description = "Athletic shirt",
            PictureUrl = "shirt.png"
        },
        new()
        {
            Id = 3,
            Name = "Nike Watch",
            Brand = "Nike",
            Type = "Accessories",
            Description = "Sports watch",
            PictureUrl = "watch.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Types = ["Shoes", "Clothing"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(x => x.Type),
            Is.EquivalentTo(["Shoes", "Clothing"]));
    }

    [Test]
    public void Constructor_WithoutSort_ShouldSetOrderByName()
    {
        var specParams = new ProductSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Sort = null
        };

        var spec = new ProductSpecifications(specParams);

        Assert.That(spec.orderBy, Is.Not.Null);
        Assert.That(spec.OrderByDescending, Is.Null);
    }

    [Test]
    public void Constructor_WithPriceAsc_ShouldSetOrderByPrice()
    {
        var specParams = new ProductSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Sort = "PriceAsc"
        };

        var spec = new ProductSpecifications(specParams);

        Assert.That(spec.orderBy, Is.Not.Null);
        Assert.That(spec.OrderByDescending, Is.Null);
    }

    [Test]
    public void Constructor_WithPriceDesc_ShouldSetOrderByDescendingPrice()
    {
        var specParams = new ProductSpecParams
        {
            PageIndex = 1,
            PageSize = 10,
            Sort = "PriceDesc"
        };

        var spec = new ProductSpecifications(specParams);

        Assert.That(spec.OrderByDescending, Is.Not.Null);
        Assert.That(spec.orderBy, Is.Null);
    }

    [Test]
    public void Constructor_WithPaging_ShouldSetSkipAndTake()
    {
        var specParams = new ProductSpecParams
        {
            PageIndex = 3,
            PageSize = 5
        };

        var spec = new ProductSpecifications(specParams);

        Assert.Multiple(() =>
        {
            Assert.That(spec.IsPagingEnabled, Is.True);
            Assert.That(spec.Skip, Is.EqualTo(10));
            Assert.That(spec.Take, Is.EqualTo(5));
        });
    }

    [Test]
    public void Constructor_WithSearchAndBrand_ShouldApplyBothFilters()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Nike Shirt",
            Brand = "Adidas",
            Type = "Clothing",
            Description = "Athletic shirt",
            PictureUrl = "shirt.png"
        },
        new()
        {
            Id = 3,
            Name = "Adidas Shoes",
            Brand = "Adidas",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "adidas.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Search = "Nike",
            Brands = ["Nike"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public void Constructor_WithSearchBrandAndType_ShouldApplyAllFilters()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Nike Shoes",
            Brand = "Nike",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.png"
        },
        new()
        {
            Id = 2,
            Name = "Nike Shirt",
            Brand = "Nike",
            Type = "Clothing",
            Description = "Athletic shirt",
            PictureUrl = "shirt.png"
        },
        new()
        {
            Id = 3,
            Name = "Adidas Shoes",
            Brand = "Adidas",
            Type = "Shoes",
            Description = "Running shoes",
            PictureUrl = "adidas.png"
        }
    };

        var specParams = new ProductSpecParams
        {
            Search = "Nike",
            Brands = ["Nike"],
            Types = ["Shoes"],
            PageIndex = 1,
            PageSize = 10
        };

        var spec = new ProductSpecifications(specParams);
        var result = spec.ApplayCriteria(products.AsQueryable()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public void ProductSpecParams_WithCommaSeparatedBrands_ShouldSplitBrands()
    {
        var specParams = new ProductSpecParams
        {
            Brands = ["Nike,Adidas"]
        };

        Assert.That(
            specParams.Brands,
            Is.EquivalentTo(new[] { "Nike", "Adidas" })
        );
    }

    [Test]
    public void ProductSpecParams_WithCommaSeparatedTypes_ShouldSplitTypes()
    {
        var specParams = new ProductSpecParams
        {
            Types = ["Shoes,Clothing"]
        };

        Assert.That(
            specParams.Types,
            Is.EquivalentTo(new[] { "Shoes", "Clothing" })
        );
    }

    [Test]
    public void ProductSpecParams_WithSearch_ShouldConvertToLowercase()
    {
        var specParams = new ProductSpecParams
        {
            Search = "NIKE Shoes"
        };

        Assert.That(specParams.Search, Is.EqualTo("nike shoes"));
    }

}
