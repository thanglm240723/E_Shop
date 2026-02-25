using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.Entities;


namespace EcommerceAPI.Core.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedAsync(
            PaginationParams paginationParams,
            int? categoryId = null,
            string? searchTerm = null);

        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
    }
}
