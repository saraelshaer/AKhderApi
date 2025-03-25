using BlogSystemApi.Validators;
using AKhderApi.Models;
using AKhderApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.CategoryDtos
{
    public class UpdateCategoryDto
    {
        public string? Name { get; set; }

        [AllowedImageFile(6)]
        public IFormFile? ImageFile { get; set; }
    }
}
