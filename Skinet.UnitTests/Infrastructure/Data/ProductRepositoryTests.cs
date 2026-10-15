using Core.Entities;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Infrastructure.Tests.Data;

[TestFixture]
public class ProductRepositoryTests
{
    private SqliteConnection _connection = null!;
    private StoreContext _context = null!;
    private ProductRepository _repository = null!;

    [SetUp]
    public async Task Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new StoreContext(options);

        await _context.Database.EnsureCreatedAsync();

        _repository = new ProductRepository(_context);
    }


    [Test]
    public async Task GetProductsAsync_FilterByBrand_ReturnsMatchingProducts()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Product A",
            Description = "Description A",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product B",
            Description = "Description B",
            PictureUrl = "b.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 200,
            QuantityInStock = 5
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync("Nike", null, null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Brand, Is.EqualTo("Nike"));
    }


    [Test]
    public async Task GetProductsAsync_FilterByType_ReturnsMatchingProducts()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Running Shoes",
            Description = "Shoes",
            PictureUrl = "shoes.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "T-Shirt",
            Description = "Shirt",
            PictureUrl = "shirt.jpg",
            Type = "Clothing",
            Brand = "Nike",
            Price = 200,
            QuantityInStock = 5
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync(null, "Shoes", null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Type, Is.EqualTo("Shoes"));
    }

    [Test]
    public async Task GetProductsAsync_SortByPriceAsc_ReturnsProductsInAscendingOrder()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Product C",
            Description = "Description C",
            PictureUrl = "c.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 300,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product A",
            Description = "Description A",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product B",
            Description = "Description B",
            PictureUrl = "b.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 200,
            QuantityInStock = 5
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync(null, null, "priceAsc");

        // Assert
        Assert.That(
            result.Select(p => p.Price),
            Is.Ordered.Ascending);
    }

    [Test]
    public async Task GetProductsAsync_SortByPriceDesc_ReturnsProductsInDescendingOrder()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Product A",
            Description = "Description A",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product C",
            Description = "Description C",
            PictureUrl = "c.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 300,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product B",
            Description = "Description B",
            PictureUrl = "b.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 200,
            QuantityInStock = 5
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync(null, null, "priceDesc");

        // Assert
        Assert.That(
            result.Select(p => p.Price),
            Is.Ordered.Descending);
    }

    [Test]
    public async Task GetProductsAsync_WithUnknownSort_ReturnsProductsOrderedByName()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Zebra",
            Description = "Description",
            PictureUrl = "z.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 300,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Apple",
            Description = "Description",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Monkey",
            Description = "Description",
            PictureUrl = "m.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 200,
            QuantityInStock = 5
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync(null, null, "unknown");

        // Assert
        Assert.That(
            result.Select(p => p.Name),
            Is.Ordered.Ascending);
    }

    [Test]
    public async Task GetProductsAsync_FilterByBrandAndType_ReturnsMatchingProducts()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Nike Shoes",
            Description = "Description",
            PictureUrl = "nike.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Nike Shirt",
            Description = "Description",
            PictureUrl = "shirt.jpg",
            Type = "Clothing",
            Brand = "Nike",
            Price = 200,
            QuantityInStock = 5
        },
        new()
        {
            Name = "Adidas Shoes",
            Description = "Description",
            PictureUrl = "adidas.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 150,
            QuantityInStock = 8
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsAsync("Nike", "Shoes", null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Brand, Is.EqualTo("Nike"));
        Assert.That(result[0].Type, Is.EqualTo("Shoes"));
    }

    [Test]
    public async Task GetBrandsAsync_ReturnsDistinctBrands()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Product A",
            Description = "Description",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product B",
            Description = "Description",
            PictureUrl = "b.jpg",
            Type = "Clothing",
            Brand = "Nike",
            Price = 200,
            QuantityInStock = 5
        },
        new()
        {
            Name = "Product C",
            Description = "Description",
            PictureUrl = "c.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 150,
            QuantityInStock = 8
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBrandsAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Does.Contain("Nike"));
        Assert.That(result, Does.Contain("Adidas"));
    }

    [Test]
    public async Task GetTypeAsync_ReturnsDistinctTypes()
    {
        // Arrange
        var products = new List<Product>
    {
        new()
        {
            Name = "Product A",
            Description = "Description",
            PictureUrl = "a.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 100,
            QuantityInStock = 10
        },
        new()
        {
            Name = "Product B",
            Description = "Description",
            PictureUrl = "b.jpg",
            Type = "Shoes",
            Brand = "Adidas",
            Price = 200,
            QuantityInStock = 5
        },
        new()
        {
            Name = "Product C",
            Description = "Description",
            PictureUrl = "c.jpg",
            Type = "Clothing",
            Brand = "Nike",
            Price = 150,
            QuantityInStock = 8
        }
    };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTypeAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Does.Contain("Shoes"));
        Assert.That(result, Does.Contain("Clothing"));
    }


    [Test]
    public async Task GetProductByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Nike Shoes",
            Description = "Running shoes",
            PictureUrl = "nike.jpg",
            Type = "Shoes",
            Brand = "Nike",
            Price = 300,
            QuantityInStock = 10
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductByIdAsync(product.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(product.Id));
        Assert.That(result.Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingId = 999;

        // Act
        var result = await _repository.GetProductByIdAsync(nonExistingId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}