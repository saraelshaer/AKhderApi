using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class User : IdentityUser
    {
       
        public bool IsActive { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public string ImageFileName { get; set; } = string.Empty;
        //-------------
        [ForeignKey("Wishlist")]
        public int? WishlistId { get; set; }
        public virtual Wishlist Wishlist { get; set; }
        //-------------
        public virtual ICollection<Review> Reviews { get; set; }
        //-------------
        [ForeignKey("Cart")]
        public int? CartId { get; set; }
        public virtual Cart Cart { get; set; }

        //-------------
        public virtual ICollection<Order> Orders { get; set; }
    }
}
