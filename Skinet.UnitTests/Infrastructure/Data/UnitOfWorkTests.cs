using Core.Entities;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Infrastructure.Tests.Data;

[TestFixture]
public class UnitOfWorkTests
{
    private SqliteConnection _connection = null!;
    private StoreContext _context = null!;
    private UnitOfWork _unitOfWork = null!;

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

        _unitOfWork = new UnitOfWork(_context);
    }


    [Test]
    public async Task Complete_WhenChangesExist_ReturnsTrue()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            PictureUrl = "test.jpg",
            Type = "Test Type",
            Brand = "Test Brand",
            Price = 100,
            QuantityInStock = 10
        };

        _context.Products.Add(product);

        // Act
        var result = await _unitOfWork.Complete();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Complete_WhenNoChangesExist_ReturnsFalse()
    {
        // Act
        var result = await _unitOfWork.Complete();

        // Assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void Repository_WhenCalledTwiceForSameEntity_ReturnsSameInstance()
    {
        // Act
        var firstRepository = _unitOfWork.Repository<Product>();
        var secondRepository = _unitOfWork.Repository<Product>();

        // Assert
        Assert.That(secondRepository, Is.SameAs(firstRepository));
    }


    [TearDown]
    public async Task TearDown()
    {
        _unitOfWork.Dispose();
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}