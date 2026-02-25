

using EcommerceAPI.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Core.Entities
{
    public class Product :BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }
       
        [MaxLength(200)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
