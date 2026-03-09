using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lấy tất cả sản phẩm (có phân trang)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ProductResponse>>>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? searchTerm = null)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _productService.GetPaginatedProductsAsync(
            paginationParams,
            categoryId,
            searchTerm);

        return Ok(ApiResponse<PaginatedResponse<ProductResponse>>.SuccessResult(
            result,
            "Lấy danh sách sản phẩm thành công"));
    }

    /// <summary>
    /// Lấy chi tiết 1 sản phẩm
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound(ApiResponse<ProductResponse>.FailureResult(
                "Không tìm thấy sản phẩm"));
        }

        return Ok(ApiResponse<ProductResponse>.SuccessResult(
            product,
            "Lấy thông tin sản phẩm thành công"));
    }

    /// <summary>
    /// Lấy sản phẩm theo danh mục
    /// </summary>
    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);

        return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResult(
            products,
            "Lấy sản phẩm theo danh mục thành công"));
    }
}