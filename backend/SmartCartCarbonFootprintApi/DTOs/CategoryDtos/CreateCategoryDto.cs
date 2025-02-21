using BlogSystemApi.Validators;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.CategoryDtos
{
    public class CreateCategoryDto
    {
        [Required]
        [Unique<Category>("Name")]
        public string Name { get; set; }

        [Required]
        [AllowedImageFile(6)]
        public IFormFile ImageFile { get; set; }
    }
}
