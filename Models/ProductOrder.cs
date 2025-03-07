using System.ComponentModel.DataAnnotations;

namespace AKhderApi.Models
{
    public class ProductOrder
    {
            public string ProductId { get; set; }
            public virtual Product Product { get; set; }

            public int OrderId { get; set; }
            public virtual Order Order { get; set; }

            [Required]
            public int Quantity { get; set; }

    }
}
