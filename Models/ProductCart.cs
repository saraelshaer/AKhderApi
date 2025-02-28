namespace SmartCartCarbonFootprintApi.Models
{
    public class ProductCart
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }

}
