namespace SmartCartCarbonFootprintApi.DTOs.ProductDtos
{
    public class ReadProductDto:BaseProductDto
    {
        public string Id { get; set; }
        public string ImagePath { get; set; }
        public string QRCodePath { get; set; } = "";
        public string CategoryName { get; set; }
         
    }
}
