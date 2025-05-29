using AKhderApi.Models;
using AKhderApi.Validators;

namespace AKhderApi.backend.DTOs.DiscountDto
{
    public class UpdateDiscountDto
    {
        [Unique<Discount>("Percentage")]
        public decimal? Percentage { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}
