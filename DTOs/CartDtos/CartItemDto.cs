using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.DTOs.CartDtos
{
    public class CartItemDto
    {
        public ReadProductDto Product { get; set; }
        public int Quantity { get; set; }
       
    }
}
