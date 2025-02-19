using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Receipt
    {
        [Key]
        public string Id { get; set; }
        public DateTime Date { get; set; }

        public string PaymentMethod { get; set; }
        //-------------
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
