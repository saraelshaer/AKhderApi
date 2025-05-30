using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AKhderApi.Models;
using AKhderApi.Validators;

namespace AKhderApi.DTOs.ProductDtos
{
    public class BaseProductDto
    {
        [Required]
        [Unique<Product>("Id")]
        public string Id { get; set; }

        [Required , MaxLength(225)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public decimal Price { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Weight must be 0 or greater.")]
        [Required]
        public decimal Weight { get; set; }

        [Required]
        public decimal CarbonFootprint { get; set; }

        [Required]
        [Range(0,double.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater.")]
        public int StockQuantity { get; set; } = 0;

        [Required]
        public int CategoryId { get; set; }

        public int? DiscountId { get; set; }
    }
}
