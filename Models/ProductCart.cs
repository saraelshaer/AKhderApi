using System.ComponentModel.DataAnnotations;

namespace AKhderApi.Models
{
    public class ProductCart
    {
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

}
