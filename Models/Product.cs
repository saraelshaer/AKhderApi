using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(225)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public decimal CarbonFootprint { get; set; }  
        public string? QRCode { get; set; }  
        public int StockQuantity { get; set; }

        public decimal Agriculture { get; set; }
        public decimal Iluc { get; set; }
        public decimal FoodProcessing { get; set; }
        public decimal Packaging { get; set; }
        public decimal Transport { get; set; }
        public decimal Retail { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        public string? ImagePath{ get; set; }

      
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [ForeignKey("Discount")]
        public int? DiscountId { get; set; }
        public virtual Discount Discount { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>(); 
        public virtual ICollection<ProductWishlist> ProductWishlists { get; set; } = new List<ProductWishlist>();
        public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
        public virtual ICollection<ProductCart>ProductCarts { get; set; } = new List<ProductCart>();

    }
}
