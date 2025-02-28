using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.AuthDtos
{
    public class TokenRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
