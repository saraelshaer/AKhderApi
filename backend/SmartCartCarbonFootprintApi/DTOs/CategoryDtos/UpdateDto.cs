using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.CategoryDtos
{
    public class UpdateDto
    {
        [Required]
        public string Name { get; set; }

        public IFormFile? ImagePath { get; set; }
    }
}
