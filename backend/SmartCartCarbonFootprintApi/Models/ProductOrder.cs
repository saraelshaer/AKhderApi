namespace SmartCartCarbonFootprintApi.Models
{
    public class ProductOrder
    {
        
            public string ProductId { get; set; }
            public Product Product { get; set; }

            public int OrderId { get; set; }
            public Order Order { get; set; }
        

    }
}
