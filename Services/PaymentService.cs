using AKhderApi.Context;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            //_context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
