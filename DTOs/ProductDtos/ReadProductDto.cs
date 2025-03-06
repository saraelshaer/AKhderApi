namespace AKhderApi.DTOs.ProductDtos
{
    public class ReadProductDto:BaseProductDto
    {
        public string Id { get; set; }
        public string ImagePath { get; set; }
        public string CategoryName { get; set; }

        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice { get; set; }

    }
}
