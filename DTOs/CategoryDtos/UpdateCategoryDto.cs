using BlogSystemApi.Validators;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.CategoryDtos
{
    public class UpdateCategoryDto
    {
        public string? Name { get; set; }

        [AllowedImageFile(6)]
        public IFormFile? ImageFile { get; set; }
    }
}
