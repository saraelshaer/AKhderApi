using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKhderApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime ReviewDate { get; set; }= DateTime.Now;
        [Range(1,5)]
        public int Rating { get; set; }
        public string Comment { get; set; }

        [ForeignKey("User")]
        public string UserId {  get; set; }
        public virtual User User { get; set; }
     
        [ForeignKey("Product")]
        public string ProductId {  get; set; }
        public virtual Product Product { get; set; }

    }
}
