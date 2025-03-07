using BlogSystemApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.UserDtos
{
    public class UpdateUserProfileDto
    {

        [EmailAddress]
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? PhoneNumber { get; set; }

        [AllowedImageFile(6)]
        public IFormFile? Imagefile { get; set; }

        public string? ImageFileName { get; set; }
    }
}
