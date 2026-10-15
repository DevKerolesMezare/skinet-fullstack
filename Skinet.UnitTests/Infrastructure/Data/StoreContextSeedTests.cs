
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Infrastructure.Tests.Data;

[TestFixture]
public class StoreContextSeedTests
{
    private SqliteConnection _connection = null!;
    private StoreContext _context = null!;
    private ServiceProvider _serviceProvider = null!;
    private UserManager<AppUser> _userManager = null!;

    [SetUp]
    public async Task Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<StoreContext>(
            options => options.UseSqlite(_connection));

        services.AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StoreContext>();

        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<StoreContext>();
        _userManager = _serviceProvider
            .GetRequiredService<UserManager<AppUser>>();

        await _context.Database.EnsureCreatedAsync();

        var roleManager = _serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    [Test]
    public async Task SeedAsync_WhenProductsExist_DoesNotAddProductsAgain()
    {
        _context.Products.Add(new Product
        {
            Name = "Existing Product",
            Description = "Test",
            PictureUrl = "test.jpg",
            Price = 10,
            Brand = "Test",
            Type = "Test"
        });

        await _context.SaveChangesAsync();

        var countBefore = await _context.Products.CountAsync();

        await StoreContextSeed.SeedAsync(_context, _userManager);

        Assert.That(
            await _context.Products.CountAsync(),
            Is.EqualTo(countBefore));
    }

    [Test]
    public async Task SeedAsync_WhenProductsAreEmpty_SeedsProducts()
    {
        await StoreContextSeed.SeedAsync(_context, _userManager);

        Assert.That(
            await _context.Products.CountAsync(),
            Is.GreaterThan(0));
    }

    [Test]
    public async Task SeedAsync_WhenDeliveryMethodsAreEmpty_SeedsDeliveryMethods()
    {
        await StoreContextSeed.SeedAsync(_context, _userManager);

        Assert.That(
            await _context.DeliveryMethods.CountAsync(),
            Is.GreaterThan(0));
    }

    [Test]
    public async Task SeedAsync_WhenAdminDoesNotExist_CreatesAdmin()
    {
        await StoreContextSeed.SeedAsync(_context, _userManager);

        var admin = await _userManager.FindByEmailAsync("admin@test.com");

        Assert.That(admin, Is.Not.Null);
        Assert.That(admin!.UserName, Is.EqualTo("admin@test.com"));
    }

    [Test]
    public async Task SeedAsync_WhenAdminExists_DoesNotCreateAdminAgain()
    {
        var admin = new AppUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com"
        };

        var result = await _userManager.CreateAsync(admin, "Pa$$w0rd");
        Assert.That(result.Succeeded, Is.True);

        await StoreContextSeed.SeedAsync(_context, _userManager);

        var adminCount = await _userManager.Users.CountAsync(
            x => x.UserName == "admin@test.com");

        Assert.That(adminCount, Is.EqualTo(1));
    }

    [TearDown]
    public async Task TearDown()
    {
        _userManager.Dispose();
        await _context.DisposeAsync();
        await _serviceProvider.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
