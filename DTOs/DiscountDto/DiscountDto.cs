using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.backend.DTOs.DiscountDto
{
    public class DiscountDto
    {

        [Required]
        [Unique<Discount>("Percentage")]
        public decimal Percentage { get; set; }

        [Required]
        public DateOnly ExpiryDate { get; set; }
    }
}
