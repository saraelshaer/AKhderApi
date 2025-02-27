using Microsoft.Build.Framework;

namespace SmartCartCarbonFootprintApi.DTOs.AuthDtos
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
