using AKhderApi.Consts;
using AKhderApi.DTOs.CartDtos;
using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.OrderDtos
{
    public class ReadOrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalCarbonFootprint { get; set; }

        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus TransactionStatus { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        public IEnumerable<CartItemDto> OrderItems { get; set; }
    }
}
