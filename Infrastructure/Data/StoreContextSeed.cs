using System.Reflection;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!context.Products.Any())
        {
            var productsData = await File
                .ReadAllTextAsync(path + @"/Data/SeedData/products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
            var DeleveryMethodData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
            var methodes = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeleveryMethodData);

            if (methodes == null) return;

            context.DeliveryMethods.AddRange(methodes);

            await context.SaveChangesAsync();
        }

    }
}
