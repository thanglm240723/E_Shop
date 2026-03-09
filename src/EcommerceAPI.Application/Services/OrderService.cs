using AutoMapper;
using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.DTOs.Requests;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Entities;
using EcommerceAPI.Core.Enums;
using EcommerceAPI.Core.Interfaces.Repositories;
using EcommerceAPI.Core.Interfaces.Services;

namespace EcommerceAPI.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IEmailService emailService,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // Validate: Kiểm tra sản phẩm tồn tại và đủ hàng
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = new List<Product>();

        foreach (var productId in productIds)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new Exception($"Sản phẩm ID {productId} không tồn tại hoặc không còn bán.");
            }

            var requestedQuantity = request.Items.First(i => i.ProductId == productId).Quantity;
            if (product.Stock < requestedQuantity)
            {
                throw new Exception($"Sản phẩm '{product.ProductName}' không đủ hàng. Còn lại: {product.Stock}");
            }

            products.Add(product);
        }

        // Tạo Order
        var order = new Order
        {
            OrderCode = GenerateOrderCode(),
            CustomerName = request.CustomerName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Note = request.Note,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Tạo OrderDetails và tính tổng tiền
        decimal totalAmount = 0;
        var orderDetails = new List<OrderDetail>();

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var subTotal = product.Price * item.Quantity;

            var orderDetail = new OrderDetail
            {
                ProductId = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = item.Quantity,
                SubTotal = subTotal
            };

            orderDetails.Add(orderDetail);
            totalAmount += subTotal;

            // Trừ stock (optional - có thể làm sau khi xác nhận đơn)
            product.Stock -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;
        order.OrderDetails = orderDetails;

        // Lưu vào database
        await _orderRepository.AddAsync(order);

        // Map to response
        var orderResponse = _mapper.Map<OrderResponse>(order);

        // Gửi email (không chờ - fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendOrderConfirmationEmailAsync(orderResponse);
                await _emailService.SendOrderNotificationToAdminAsync(orderResponse);
            }
            catch (Exception ex)
            {
                // Log error nhưng không throw - không làm fail order
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        });

        return orderResponse;
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);

        if (order == null)
            return null;

        return _mapper.Map<OrderResponse>(order);
    }

    public async Task<OrderResponse?> GetOrderByCodeAsync(string orderCode)
    {
        var order = await _orderRepository.GetByOrderCodeAsync(orderCode);

        if (order == null)
            return null;

        return _mapper.Map<OrderResponse>(order);
    }

    public async Task<PaginatedResponse<OrderResponse>> GetPaginatedOrdersAsync(
        PaginationParams paginationParams,
        int? status = null)
    {
        var (orders, totalCount) = await _orderRepository.GetPaginatedAsync(paginationParams, status);

        var orderResponses = _mapper.Map<List<OrderResponse>>(orders);

        return new PaginatedResponse<OrderResponse>(
            orderResponses,
            totalCount,
            paginationParams.PageNumber,
            paginationParams.PageSize);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, int status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return false;

        order.Status = (OrderStatus)status;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        return true;
    }

    private string GenerateOrderCode()
    {
        // Format: ORD20240216001
        var date = DateTime.Now.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"ORD{date}{random}";
    }
}