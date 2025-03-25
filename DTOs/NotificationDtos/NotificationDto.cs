using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.NotificationDtos
{
    public class NotificationDto
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }
   
    }
}
