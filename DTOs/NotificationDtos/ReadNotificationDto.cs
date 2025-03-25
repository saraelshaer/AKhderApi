namespace AKhderApi.DTOs.NotificationDtos
{
    public class ReadNotificationDto: NotificationDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } 
    }
}
