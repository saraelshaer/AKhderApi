using System.ComponentModel.DataAnnotations;

namespace AKhderApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsGeneral { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();


    }
}
