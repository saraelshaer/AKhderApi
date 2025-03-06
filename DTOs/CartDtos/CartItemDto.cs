using AKhderApi.DTOs.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.CartDtos
{
    public class CartItemDto
    {
        public ReadProductDto Product { get; set; }
        public int Quantity { get; set; }
       
    }
}
