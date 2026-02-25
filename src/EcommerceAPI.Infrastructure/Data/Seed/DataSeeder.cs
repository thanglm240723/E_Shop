using EcommerceAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { CategoryName = "Điện thoại", Description = "Điện thoại di động", IsActive = true },
                new() { CategoryName = "Laptop", Description = "Máy tính xách tay", IsActive = true },
                new() { CategoryName = "Phụ kiện", Description = "Phụ kiện công nghệ", IsActive = true }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            var phoneCategory = await context.Categories.FirstAsync(c => c.CategoryName == "Điện thoại");

            var products = new List<Product>
            {
                new()
                {
                    ProductName = "iPhone 15 Pro Max",
                    Description = "Điện thoại cao cấp từ Apple",
                    Price = 29990000,
                    Stock = 50,
                    CategoryId = phoneCategory.Id,
                    IsActive = true,
                    ImageUrl = "https://via.placeholder.com/300"
                },
                new()
                {
                    ProductName = "Samsung Galaxy S24 Ultra",
                    Description = "Flagship từ Samsung",
                    Price = 27990000,
                    Stock = 30,
                    CategoryId = phoneCategory.Id,
                    IsActive = true,
                    ImageUrl = "https://via.placeholder.com/300"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}