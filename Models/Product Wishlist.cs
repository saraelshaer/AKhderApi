namespace AKhderApi.Models
{
    
        public class ProductWishlist
        {
            public string ProductId { get; set; }
            public virtual Product Product { get; set; }

            public int WishlistId { get; set; }
            public virtual Wishlist Wishlist { get; set; }
        }

    
}
