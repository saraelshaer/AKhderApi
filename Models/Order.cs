using AKhderApi.Consts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AKhderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public double TotalCarbonFootprint { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        [EnumDataType(typeof(TransactionStatus))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;

        [ForeignKey("User")]
        public string UserId {  get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    }
}
