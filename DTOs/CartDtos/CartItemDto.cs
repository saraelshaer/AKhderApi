using AKhderApi.DTOs.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.CartDtos
{
    public class CartItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal CarbonFootprint { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
       
    }
}
