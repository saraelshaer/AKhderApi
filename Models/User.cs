using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.Models
{
    public class User: IdentityUser
    {
       
        public bool IsActive { get; set; } = true;

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public string ImageFileName { get; set; } = "/Images/defaultImage.svg";

        public virtual Wishlist Wishlist { get; set; }

        public virtual Cart Cart { get; set; }

        public virtual ICollection<Review>Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
