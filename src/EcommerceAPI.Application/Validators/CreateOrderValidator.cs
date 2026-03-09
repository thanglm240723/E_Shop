using EcommerceAPI.Core.DTOs.Requests;
using FluentValidation;

namespace EcommerceAPI.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Tên khách hàng không được để trống")
            .MinimumLength(2).WithMessage("Tên khách hàng phải có ít nhất 2 ký tự")
            .MaximumLength(100).WithMessage("Tên khách hàng không được quá 100 ký tự");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Số điện thoại không hợp lệ (VD: 0901234567)");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống")
            .MinimumLength(10).WithMessage("Địa chỉ quá ngắn, vui lòng nhập đầy đủ")
            .MaximumLength(500).WithMessage("Địa chỉ không được quá 500 ký tự");

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Ghi chú không được quá 1000 ký tự");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Đơn hàng phải có ít nhất 1 sản phẩm")
            .Must(items => items.Count > 0).WithMessage("Đơn hàng phải có ít nhất 1 sản phẩm");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID không hợp lệ");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(100).WithMessage("Số lượng không được vượt quá 100");
        });
    }
}