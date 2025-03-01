using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Validators;

namespace SmartCartCarbonFootprintApi.DTOs.DiscountDto
{
    public class UpdateDiscountDto
    {
        [Unique<Discount>("Percentage")]
        public decimal? Percentage { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}
