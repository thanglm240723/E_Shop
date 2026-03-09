

using EcommerceAPI.Core.DTOs.Responses;

namespace EcommerceAPI.Application.Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<IEnumerable<ProductResponse>> GetProductsByIdAsync(int Id);
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId);
    }
}
