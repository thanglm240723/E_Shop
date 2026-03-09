using AutoMapper;
using EcommerceAPI.Core.DTOs.Common;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Repositories;
using EcommerceAPI.Core.Interfaces.Services;

namespace EcommerceAPI.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null || !product.IsActive)
            return null;    

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<PaginatedResponse<ProductResponse>> GetPaginatedProductsAsync(
        PaginationParams paginationParams,
        int? categoryId = null,
        string? searchTerm = null)
    {
        var (products, totalCount) = await _productRepository.GetPaginatedAsync(
            paginationParams,
            categoryId,
            searchTerm);

        var productResponses = _mapper.Map<List<ProductResponse>>(products);

        return new PaginatedResponse<ProductResponse>(
            productResponses,
            totalCount,
            paginationParams.PageNumber,
            paginationParams.PageSize);
    }

    public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
}