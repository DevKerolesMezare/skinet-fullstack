
using Core.Entities.OrderAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Skinet.UnitTests.Infrastructure.Config;

[TestFixture]
public class OrderItemConfigurationTests
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
    public void Configure_ItemOrdered_IsOwned()
    {
        var entity = _context.Model.FindEntityType(typeof(OrderItem))!;

        var navigation = entity.FindNavigation(nameof(OrderItem.ItemOrdered));

        Assert.That(navigation, Is.Not.Null);
        Assert.That(navigation!.TargetEntityType.IsOwned(), Is.True);
    }

    [Test]
    public void Configure_Price_HasCorrectColumnType()
    {
        var property = _context.Model
            .FindEntityType(typeof(OrderItem))!
            .FindProperty(nameof(OrderItem.Price));

        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetColumnType(), Is.EqualTo("decimal(18,2)"));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
