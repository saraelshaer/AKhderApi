using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.DTOs.ProductDtos
{
    public class ProductPaginationDto
    {
        public int TotalProductsCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

        public IEnumerable<ReadProductDto> Products { get; set; }

    }
}
