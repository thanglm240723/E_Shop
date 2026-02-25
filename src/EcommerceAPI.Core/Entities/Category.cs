

using EcommerceAPI.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Core.Entities
{
    public class Category : BaseEntity
    {

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
