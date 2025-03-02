using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class User: IdentityUser
    {
       
        public bool IsActive { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public string ImageFileName { get; set; } = string.Empty;

        public virtual Wishlist Wishlist { get; set; }

        public virtual Cart Cart { get; set; }

        public virtual ICollection<Review>Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
