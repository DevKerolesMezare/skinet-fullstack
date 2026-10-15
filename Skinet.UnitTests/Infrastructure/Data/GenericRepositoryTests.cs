using Core.Entities;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Skinet.UnitTests.Infrastructure.Data;

[TestFixture]
public class GenericRepositoryTests
{
    public class ProductNameSpecification : BaseSpecification<Product, string>
    {
        public ProductNameSpecification() : base(p => true)
        {
            AddSelect(p => p.Name);
        }
    }

    private SqliteConnection _connection = null!;
    private StoreContext _context = null!;
    private GenericRepository<Product> _repository = null!;

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

        _repository = new GenericRepository<Product>(_context);
    }


    [Test]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(product.Id));
        Assert.That(result.Name, Is.EqualTo(product.Name));

    }


    [Test]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingId = 999;

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        Assert.That(result, Is.Null);
    }



    [Test]
    public async Task Exists_WhenEntityExists_ReturnsTrue()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = _repository.Exists(product.Id);

        // Assert
        Assert.That(result, Is.True);
    }


    [Test]
    public void Exists_WhenEntityDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistingId = 999;

        // Act
        var result = _repository.Exists(nonExistingId);

        // Assert
        Assert.That(result, Is.False);
    }


    [Test]
    public async Task ListAllAsync_ReturnsAllEntities()
    {
        // Arrange
        _context.Products.AddRange(
        new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        },
        new Product
        {
            Name = "Phone",
            Description = "Smartphone",
            Price = 800,
            PictureUrl = "phone.jpg",
            Type = "Electronics",
            Brand = "Samsung",
            QuantityInStock = 5
        }
    );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }


    [Test]
    public void Add_AddsEntityToContext()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        // Act
        _repository.Add(product);

        // Assert
        Assert.That(_context.Entry(product).State, Is.EqualTo(EntityState.Added));
    }



    [Test]
    public async Task Remove_RemovesEntityFromContext()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(product);

        // Assert
        Assert.That(_context.Entry(product).State, Is.EqualTo(EntityState.Deleted));
    }



    [Test]
    public async Task Update_ChangesEntityStateToModified()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        _repository.Update(product);

        // Assert
        Assert.That(_context.Entry(product).State, Is.EqualTo(EntityState.Modified));
    }



    [Test]
    public async Task CountAsync_ReturnsMatchingCount()
    {
        // Arrange
        _context.Products.AddRange(
            new Product
            {
                Name = "Laptop",
                Description = "Gaming laptop",
                Price = 1500,
                PictureUrl = "laptop.jpg",
                Type = "Electronics",
                Brand = "Lenovo",
                QuantityInStock = 10
            },
            new Product
            {
                Name = "Phone",
                Description = "Smartphone",
                Price = 800,
                PictureUrl = "phone.jpg",
                Type = "Electronics",
                Brand = "Samsung",
                QuantityInStock = 5
            }
        );

        await _context.SaveChangesAsync();

        var spec = new BaseSpecification<Product>(p => p.Price > 1000);

        // Act
        var result = await _repository.CountAsync(spec);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }


    [Test]
    public async Task ListAsync_ReturnsMatchingEntities()
    {
        // Arrange
        _context.Products.AddRange(
            new Product
            {
                Name = "Laptop",
                Description = "Gaming laptop",
                Price = 1500,
                PictureUrl = "laptop.jpg",
                Type = "Electronics",
                Brand = "Lenovo",
                QuantityInStock = 10
            },
            new Product
            {
                Name = "Phone",
                Description = "Smartphone",
                Price = 800,
                PictureUrl = "phone.jpg",
                Type = "Electronics",
                Brand = "Samsung",
                QuantityInStock = 5
            }
        );

        await _context.SaveChangesAsync();

        var spec = new BaseSpecification<Product>(p => p.Price > 1000);

        // Act
        var result = await _repository.ListAsync(spec);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }


    [Test]
    public async Task GetEntityWithSpec_ReturnsMatchingEntity()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var spec = new BaseSpecification<Product>(p => p.Price > 1000);

        // Act
        var result = await _repository.GetEntityWithSpec(spec);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(product.Id));
    }



    [Test]
    public async Task GetEntityWithSpec_WhenNoEntityMatches_ReturnsNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "Phone",
            Description = "Smartphone",
            Price = 800,
            PictureUrl = "phone.jpg",
            Type = "Electronics",
            Brand = "Samsung",
            QuantityInStock = 5
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var spec = new BaseSpecification<Product>(p => p.Price > 1000);

        // Act
        var result = await _repository.GetEntityWithSpec(spec);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task ListAsync_WithProjection_ReturnsSelectedValues()
    {
        // Arrange
        _context.Products.AddRange(
            new Product
            {
                Name = "Laptop",
                Description = "Gaming laptop",
                Price = 1500,
                PictureUrl = "laptop.jpg",
                Type = "Electronics",
                Brand = "Lenovo",
                QuantityInStock = 10
            },
            new Product
            {
                Name = "Phone",
                Description = "Smartphone",
                Price = 800,
                PictureUrl = "phone.jpg",
                Type = "Electronics",
                Brand = "Samsung",
                QuantityInStock = 5
            }
        );

        await _context.SaveChangesAsync();

        var spec = new ProductNameSpecification();

        // Act
        var result = await _repository.ListAsync<string>(spec);

        // Assert
        Assert.That(result, Is.EquivalentTo(new[] { "Laptop", "Phone" }));
    }


    [Test]
    public async Task GetEntityWithSpec_WithProjection_ReturnsSelectedValue()
    {
        // Arrange
        var product = new Product
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500,
            PictureUrl = "laptop.jpg",
            Type = "Electronics",
            Brand = "Lenovo",
            QuantityInStock = 10
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var spec = new ProductNameSpecification();

        // Act
        var result = await _repository.GetEntityWithSpec<string>(spec);

        // Assert
        Assert.That(result, Is.EqualTo("Laptop"));
    }



    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
