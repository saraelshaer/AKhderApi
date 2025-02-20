using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
       
        //-------------
     
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public ICollection<ProductWishlist> ProductWishlists { get; set; }
    }
}
