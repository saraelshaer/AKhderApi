using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Product
    {
        public int Id { get; set; }  

        public string Name { get; set; } 

        public decimal Price { get; set; } 

        public double Weight { get; set; }  

        public double CarbonFootprint { get; set; }  

        public string QRCode { get; set; }  

        public int StockQuantity { get; set; }  

        public string Description { get; set; }  

        public bool IsActive { get; set; } 

        public string ImagePath{ get; set; }

      
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        //-------------
        public virtual ICollection<Review> Reviews { get; set; }
        //-------------
        public virtual ICollection<ProductWishlist> ProductWishlists { get; set; } = new List<ProductWishlist>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } 
        //-------------
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        //-------------
        public virtual ICollection<ProductCart>ProductCarts { get; set; } = new List<ProductCart>();
        public virtual ICollection<Cart> Carts { get; set; }

    }
}
