using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Category
    {
        public int Id { get; set; }  

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public string ImagePath { get; set; }
      
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();   
    }
}
