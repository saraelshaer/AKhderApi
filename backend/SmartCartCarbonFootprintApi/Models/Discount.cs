namespace SmartCartCarbonFootprintApi.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public decimal Percentage { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
