using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Lấy tất cả danh mục
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponse>>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        return Ok(ApiResponse<IEnumerable<CategoryResponse>>.SuccessResult(
            categories,
            "Lấy danh sách danh mục thành công"));
    }

    /// <summary>
    /// Lấy chi tiết 1 danh mục
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound(ApiResponse<CategoryResponse>.FailureResult(
                "Không tìm thấy danh mục"));
        }

        return Ok(ApiResponse<CategoryResponse>.SuccessResult(
            category,
            "Lấy thông tin danh mục thành công"));
    }
}