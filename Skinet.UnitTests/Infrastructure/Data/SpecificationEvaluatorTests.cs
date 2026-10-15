
using Core.Entities;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Infrastructure.Tests.Data;

[TestFixture]
public class SpecificationEvaluatorTests
{
    private SqliteConnection _connection = null!;
    private StoreContext _context = null!;

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
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Test]
    public async Task GetQuery_WithCriteria_FiltersResults()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Nike Shoes", 300, "Nike"),
            CreateProduct("Adidas Shoes", 200, "Adidas"));

        await _context.SaveChangesAsync();

        var spec = new ProductNameSpecification("Nike Shoes");

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike Shoes"));
    }

    [Test]
    public async Task GetQuery_WithOrderBy_SortsResults()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Product C", 300),
            CreateProduct("Product A", 100),
            CreateProduct("Product B", 200));

        await _context.SaveChangesAsync();

        var spec = new OrderByPriceSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result.Select(p => p.Price), Is.Ordered.Ascending);
    }

    [Test]
    public async Task GetQuery_WithOrderByDescending_SortsResultsDescending()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Product A", 100),
            CreateProduct("Product B", 200),
            CreateProduct("Product C", 300));

        await _context.SaveChangesAsync();

        var spec = new OrderByDescendingPriceSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result.Select(p => p.Price), Is.Ordered.Descending);
    }

    [Test]
    public async Task GetQuery_WithSelect_ReturnsProjectedResults()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Nike Shoes", 300),
            CreateProduct("Adidas Shoes", 200));

        await _context.SaveChangesAsync();

        var spec = new ProductNameTestSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery<string>(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Does.Contain("Nike Shoes"));
        Assert.That(result, Does.Contain("Adidas Shoes"));
    }

    [Test]
    public async Task GetQuery_WithPaging_ReturnsRequestedPage()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Product A", 100),
            CreateProduct("Product B", 200),
            CreateProduct("Product C", 300));

        await _context.SaveChangesAsync();

        var spec = new PagingProductTestSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Product B"));
    }

    [Test]
    public async Task GetQuery_WithDistinct_RemovesDuplicateProjectedValues()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Product A", 100, "Nike"),
            CreateProduct("Product B", 200, "Nike"),
            CreateProduct("Product C", 300, "Adidas"));

        await _context.SaveChangesAsync();

        var spec = new DistinctBrandTestSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery<string>(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Does.Contain("Nike"));
        Assert.That(result, Does.Contain("Adidas"));
    }

    [Test]
    public async Task GetQuery_WithCriteriaAndPaging_ReturnsFilteredPage()
    {
        // Arrange
        _context.Products.AddRange(
            CreateProduct("Nike A", 100, "Nike"),
            CreateProduct("Nike B", 200, "Nike"),
            CreateProduct("Adidas A", 300, "Adidas"));

        await _context.SaveChangesAsync();

        var spec = new NikePagingTestSpecification();

        // Act
        var result = await SpecificationEvaluator<Product>
            .GetQuery(_context.Products, spec)
            .ToListAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Nike B"));
    }

    private static Product CreateProduct(
        string name,
        decimal price,
        string brand = "Brand")
    {
        return new Product
        {
            Name = name,
            Description = $"Description for {name}",
            PictureUrl = $"{name.Replace(" ", "-")}.jpg",
            Brand = brand,
            Type = "Shoes",
            Price = price,
            QuantityInStock = 10
        };
    }

    private class ProductNameSpecification : BaseSpecification<Product>
    {
        public ProductNameSpecification(string name)
            : base(p => p.Name == name)
        {
        }
    }

    private class OrderByPriceSpecification : BaseSpecification<Product>
    {
        public OrderByPriceSpecification()
        {
            AddOrderBy(p => p.Price);
        }
    }

    private class OrderByDescendingPriceSpecification
        : BaseSpecification<Product>
    {
        public OrderByDescendingPriceSpecification()
        {
            AddOrderByDescending(p => p.Price);
        }
    }

    private class ProductNameTestSpecification
        : BaseSpecification<Product, string>
    {
        public ProductNameTestSpecification()
        {
            AddSelect(product => product.Name);
        }
    }

    private class PagingProductTestSpecification : BaseSpecification<Product>
    {
        public PagingProductTestSpecification()
        {
            AddOrderBy(p => p.Price);
            ApplyPaging(1, 1);
        }
    }

    private class DistinctBrandTestSpecification
        : BaseSpecification<Product, string>
    {
        public DistinctBrandTestSpecification()
        {
            AddSelect(product => product.Brand);
            ApplyDistinct();
        }
    }

    private class NikePagingTestSpecification : BaseSpecification<Product>
    {
        public NikePagingTestSpecification()
            : base(p => p.Brand == "Nike")
        {
            AddOrderBy(p => p.Price);
            ApplyPaging(1, 1);
        }
    }
}
