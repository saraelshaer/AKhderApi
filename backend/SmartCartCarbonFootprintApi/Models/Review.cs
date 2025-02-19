using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        //-------------

        [ForeignKey("User")]
        public int UserId {  get; set; }
        public virtual User User { get; set; }
        //-------------
        [ForeignKey("Product")]
        public int ProductId {  get; set; }
        public virtual Product Product { get; set; }

    }
}
