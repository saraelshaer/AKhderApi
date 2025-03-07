using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class UserNotification
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Notification")]
        public int NotificationId { get; set; }
        public virtual Notification Notification { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
