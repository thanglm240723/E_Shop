using AutoMapper;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Repositories;
using EcommerceAPI.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveCategoriesAsync();

        var categoryResponses = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive,
            ProductCount = c.Products?.Count ?? 0
        }).ToList();

        return categoryResponses;
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null || !category.IsActive)
            return null;

        return new CategoryResponse
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count ?? 0
        };
    }
}