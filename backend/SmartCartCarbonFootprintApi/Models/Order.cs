using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public decimal TotalPrice { get; set; }

        public double TotalWeight { get; set; }

        public double TotalCarbonFootprint { get; set; }
        public DateTime Date { get; set; }
        //-------------
        [ForeignKey("User")]
        public int UserId {  get; set; }
        public virtual User User { get; set; }
        //-------------
        public ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        //-------------
        public virtual Receipt Receipt { get; set; }
        //-------------
        [ForeignKey("Cart")]
        public int CartId {  get; set; }
        public virtual Cart Cart { get; set; }



    }
}
