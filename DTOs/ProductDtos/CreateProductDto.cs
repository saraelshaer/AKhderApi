using BlogSystemApi.Validators;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.ProductDtos
{
    public class CreateProductDto:BaseProductDto
    {

        [Required]
        [AllowedImageFile(6)]
        public IFormFile ImageFile { get; set; }

    }
}
