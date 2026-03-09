using AutoMapper;
using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Entities;
using EcommerceAPI.Core.Enums;

namespace EcommerceAPI.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Product mappings
        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));

        // Category mappings
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.ProductCount,
                opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

        // Order mappings
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.StatusText,
                opt => opt.MapFrom(src => GetOrderStatusText(src.Status)))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.OrderDetails));

        // OrderDetail mappings
        CreateMap<OrderDetail, OrderDetailResponse>();
    }

    private static string GetOrderStatusText(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "Chờ xác nhận",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }
}