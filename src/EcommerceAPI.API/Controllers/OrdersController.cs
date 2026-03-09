using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.DTOs.Requests;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Tạo đơn hàng mới (Checkout)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request);

            return Ok(ApiResponse<OrderResponse>.SuccessResult(
                order,
                "Đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn sớm."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<OrderResponse>.FailureResult(
                ex.Message));
        }
    }

    /// <summary>
    /// Tra cứu đơn hàng theo mã đơn (cho khách hàng)
    /// </summary>
    [HttpGet("track/{orderCode}")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> TrackOrder(string orderCode)
    {
        var order = await _orderService.GetOrderByCodeAsync(orderCode);

        if (order == null)
        {
            return NotFound(ApiResponse<OrderResponse>.FailureResult(
                "Không tìm thấy đơn hàng"));
        }

        return Ok(ApiResponse<OrderResponse>.SuccessResult(
            order,
            "Lấy thông tin đơn hàng thành công"));
    }

    /// <summary>
    /// Lấy chi tiết đơn hàng theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound(ApiResponse<OrderResponse>.FailureResult(
                "Không tìm thấy đơn hàng"));
        }

        return Ok(ApiResponse<OrderResponse>.SuccessResult(
            order,
            "Lấy thông tin đơn hàng thành công"));
    }

    /// <summary>
    /// Lấy danh sách đơn hàng (có phân trang, lọc theo status) - ADMIN
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<OrderResponse>>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? status = null)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _orderService.GetPaginatedOrdersAsync(paginationParams, status);

        return Ok(ApiResponse<PaginatedResponse<OrderResponse>>.SuccessResult(
            result,
            "Lấy danh sách đơn hàng thành công"));
    }

    /// <summary>
    /// Cập nhật trạng thái đơn hàng - ADMIN
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(
        int id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, request.Status);

        if (!success)
        {
            return NotFound(ApiResponse<bool>.FailureResult(
                "Không tìm thấy đơn hàng"));
        }

        return Ok(ApiResponse<bool>.SuccessResult(
            true,
            "Cập nhật trạng thái đơn hàng thành công"));
    }
}

// DTO cho update status
public class UpdateOrderStatusRequest
{
    public int Status { get; set; }
}