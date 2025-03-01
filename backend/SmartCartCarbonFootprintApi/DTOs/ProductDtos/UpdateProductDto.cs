using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.ProductDtos
{
    public class UpdateProductDto
    {

        [MaxLength(225)]
        public string? Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public decimal? Price { get; set; }

        public double? CarbonFootprint { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater.")]
        public int? StockQuantity { get; set; } 

        public int? CategoryId { get; set; } 

        public int? DiscountId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
