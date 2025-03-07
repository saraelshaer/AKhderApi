using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsGeneral { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();


    }
}
