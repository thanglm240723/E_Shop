using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Core.Entities.Base;

namespace EcommerceAPI.Core.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty; 

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }  
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; } 
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}