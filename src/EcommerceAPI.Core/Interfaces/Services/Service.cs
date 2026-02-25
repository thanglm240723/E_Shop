using EcommerceAPI.Core.DTOs;
using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.DTOs.Requests;
using EcommerceAPI.Core.DTOs.Responses;
namespace EcommerceAPI.Core.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<PaginatedResponse<ProductResponse>> GetPaginatedProductsAsync(
        PaginationParams paginationParams,
        int? categoryId = null,
        string? searchTerm = null);
    Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId);
}

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    Task<CategoryResponse?> GetCategoryByIdAsync(int id);
}

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetOrderByIdAsync(int id);
    Task<OrderResponse?> GetOrderByCodeAsync(string orderCode);
    Task<PaginatedResponse<OrderResponse>> GetPaginatedOrdersAsync(
        PaginationParams paginationParams,
        int? status = null);
    Task<bool> UpdateOrderStatusAsync(int orderId, int status);
}

public interface IEmailService
{
    Task SendOrderConfirmationEmailAsync(OrderResponse order);
    Task SendOrderNotificationToAdminAsync(OrderResponse order);
}