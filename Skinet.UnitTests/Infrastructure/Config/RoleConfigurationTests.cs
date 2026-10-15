
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Skinet.UnitTests.Infrastructure.Config;

[TestFixture]
public class RoleConfigurationTests
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

    [TestCase("1", "Admin", "ADMIN")]
    [TestCase("2", "Customer", "CUSTOMER")]
    public void Configure_HasExpectedRoles(
        string id,
        string name,
        string normalizedName)
    {
        var model = _context.GetService<IDesignTimeModel>().Model;

        var roles = model
            .FindEntityType(typeof(IdentityRole))!
            .GetSeedData();

        var role = roles.SingleOrDefault(x =>
            Equals(x[nameof(IdentityRole.Id)], id));

        Assert.That(role, Is.Not.Null);
        Assert.That(role![nameof(IdentityRole.Name)], Is.EqualTo(name));
        Assert.That(
            role[nameof(IdentityRole.NormalizedName)],
            Is.EqualTo(normalizedName));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
