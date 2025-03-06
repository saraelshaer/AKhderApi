using AKhderApi.Models;
using AKhderApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.backend.DTOs.DiscountDto
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
