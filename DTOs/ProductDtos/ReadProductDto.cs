namespace AKhderApi.DTOs.ProductDtos
{
    public class ReadProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string CategoryName { get; set; }
        public decimal CarbonFootprint { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int CategoryId { get; set; }
        public int? DiscountId { get; set; }

    }
}
