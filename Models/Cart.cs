using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();

        public decimal? ActualWeight { get; set; }
    }
}
