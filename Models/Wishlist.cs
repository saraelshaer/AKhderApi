using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<ProductWishlist> ProductWishlists { get; set; }= new List<ProductWishlist>();
    }
}
