
using Core.Entities.OrderAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Tests.Config;

[TestFixture]
public class OrderConfigurationTests
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
    public void Configure_ShippingAddress_IsOwned()
    {
        var navigation = _context.Model
            .FindEntityType(typeof(Order))!
            .FindNavigation(nameof(Order.ShippingAddress));

        Assert.That(navigation, Is.Not.Null);
        Assert.That(navigation!.TargetEntityType.IsOwned(), Is.True);
    }

    [Test]
    public void Configure_PaymentSummary_IsOwned()
    {
        var navigation = _context.Model
            .FindEntityType(typeof(Order))!
            .FindNavigation(nameof(Order.PaymentSummary));

        Assert.That(navigation, Is.Not.Null);
        Assert.That(navigation!.TargetEntityType.IsOwned(), Is.True);
    }

    [Test]
    public void Configure_Status_HasStringConversion()
    {
        var property = _context.Model
            .FindEntityType(typeof(Order))!
            .FindProperty(nameof(Order.Status));

        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetValueConverter(), Is.Not.Null);
        Assert.That(
            property.GetValueConverter()!.ProviderClrType,
            Is.EqualTo(typeof(string)));
    }

    [Test]
    public void Configure_Subtotal_HasCorrectColumnType()
    {
        var property = _context.Model
            .FindEntityType(typeof(Order))!
            .FindProperty(nameof(Order.Subtotal));

        Assert.That(property, Is.Not.Null);
        Assert.That(
            property!.GetColumnType(),
            Is.EqualTo("decimal(18,2)"));
    }

    [Test]
    public void Configure_OrderItems_UsesCascadeDelete()
    {
        var orderItemType = _context.Model
            .FindEntityType(typeof(OrderItem))!;

        var foreignKey = orderItemType.GetForeignKeys()
            .Single(x => x.PrincipalEntityType.ClrType == typeof(Order));

        Assert.That(
            foreignKey.DeleteBehavior,
            Is.EqualTo(DeleteBehavior.Cascade));
    }

    [Test]
    public void Configure_OrderDate_HasConversion()
    {
        var property = _context.Model
            .FindEntityType(typeof(Order))!
            .FindProperty(nameof(Order.OrderDate));

        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetValueConverter(), Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
