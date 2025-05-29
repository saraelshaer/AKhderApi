using BlogSystemApi.Validators;
using AKhderApi.Models;
using AKhderApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.ProductDtos
{
    public class CreateProductDto:BaseProductDto
    {

        [Required]
        [AllowedImageFile(6)]
        public IFormFile ImageFile { get; set; }

    }
}
