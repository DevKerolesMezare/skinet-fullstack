
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Infrastructure.Tests.Config;

[TestFixture]
public class ProductConfigurationTests
{
    private StoreContext _context = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new StoreContext(options);
    }

    [Test]
    public void Configure_Price_HasCorrectColumnType()
    {
        var property = _context.Model
            .FindEntityType(typeof(Product))!
            .FindProperty(nameof(Product.Price));

        Assert.That(property!.GetColumnType(), Is.EqualTo("decimal(18,2)"));
    }

    [Test]
    public void Configure_Name_IsRequired()
    {
        var property = _context.Model
            .FindEntityType(typeof(Product))!
            .FindProperty(nameof(Product.Name));

        Assert.That(property!.IsNullable, Is.False);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
