using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //-------------
        [ForeignKey("Wishlist")]
        public int WishlistId { get; set; }
        public virtual Wishlist Wishlist { get; set; }
        //-------------
        public virtual ICollection<Review>Reviews { get; set; }
        //-------------
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        //-------------
        public virtual ICollection<Role> Roles { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        //-------------
        public virtual ICollection<Order> Orders { get; set; }
    }
}
