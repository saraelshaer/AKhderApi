namespace SmartCartCarbonFootprintApi.Models
{
    
        public class ProductWishlist
        {
            public string ProductId { get; set; }
            public Product Product { get; set; }

            public int WishlistId { get; set; }
            public Wishlist Wishlist { get; set; }
        }

    
}
