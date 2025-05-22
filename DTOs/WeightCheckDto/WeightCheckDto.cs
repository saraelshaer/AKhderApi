using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.WeightCheckDto
{
    public class WeightCheckDto
    {
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public int CartId { get; set; }
    }
}
