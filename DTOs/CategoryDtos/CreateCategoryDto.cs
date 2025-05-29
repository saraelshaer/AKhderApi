using BlogSystemApi.Validators;
using AKhderApi.Models;
using AKhderApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.CategoryDtos
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
