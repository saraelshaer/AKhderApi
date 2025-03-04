namespace SmartCartCarbonFootprintApi.Models
{
    public class ProductCart
    {
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
    }

}
