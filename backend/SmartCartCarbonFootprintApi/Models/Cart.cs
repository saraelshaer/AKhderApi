using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string QRCode { get; set; }
        //-------------
        public virtual User User { get; set; }
        //-------------
        public ICollection<ProductCart> ProductCarts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        //-------------
        public virtual ICollection<Order> Orders { get; set; }
    }
}
