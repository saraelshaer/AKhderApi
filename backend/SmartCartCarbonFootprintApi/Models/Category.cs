using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }  

        public string Name { get; set; }  

        public string Description { get; set; } 

        public bool IsActive { get; set; }

        public string ImageName { get; set; }
        //-------------
        public virtual ICollection<Product> Products { get; set; }
    }
}
