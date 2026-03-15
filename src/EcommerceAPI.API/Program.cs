using EcommerceAPI.Application.Mappings;
using EcommerceAPI.Application.Services;
using EcommerceAPI.Application.Validators;
using EcommerceAPI.Core.Interfaces.Repositories;
using EcommerceAPI.Core.Interfaces.Services;
using EcommerceAPI.Infrastructure.Data;
using EcommerceAPI.Infrastructure.Data.Seed;
using EcommerceAPI.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// ADD SERVICES TO THE CONTAINER
// ============================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ecommerce API",
        Version = "v1",
        Description = "API cho hệ thống bán hàng online",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Name",
            Email = "your-email@example.com"
        }
    });
});

// ============================================
// DATABASE
// ============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// ============================================
// AUTOMAPPER
// ============================================
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// ============================================
// FLUENTVALIDATION
// ============================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

// ============================================
// REPOSITORIES
// ============================================
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

// ============================================
// SERVICES
// ============================================
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ============================================
// CORS - Cho phép React frontend kết nối
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",      // Create React App
                    "http://localhost:5173",      // Vite
                    "http://localhost:5174"       // Vite alternative
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// ============================================
// SEED DATA (Development only)
// ============================================
if (true)
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Đảm bảo database đã được tạo
            if (context.Database.CanConnect())
            {
                await DataSeeder.SeedAsync(context);
                Console.WriteLine("✓ Data seeded successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error seeding data: {ex.Message}");
        }
    }
}


// ============================================
// CONFIGURE HTTP REQUEST PIPELINE
// ============================================

if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// CORS phải đặt TRƯỚC Authorization
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

// ============================================
// RUN APPLICATION
// ============================================
Console.WriteLine("========================================");
Console.WriteLine("🚀 Ecommerce API is starting...");
Console.WriteLine("========================================");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("========================================");

app.Run();