namespace SmartCartCarbonFootprintApi.DTOs.PaymentDtos
{
    public class PaymentRequest
    {
        public string ProductName { get; set; }
        public long Amount { get; set; } // Amount in cents
        public string Currency { get; set; }
    }
}
