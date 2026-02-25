using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.Entities;


namespace EcommerceAPI.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetByOrderCodeAsync(string orderCode);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(int status);
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetPaginatedAsync(
            PaginationParams paginationParams,
            int? status = null);
    }
}
