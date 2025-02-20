using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.CategoryDtos
{
    public class CreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile ImagePath { get; set; }
    }
}
