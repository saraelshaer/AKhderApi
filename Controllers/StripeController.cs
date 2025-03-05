using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartCartCarbonFootprintApi.DTOs.PaymentDtos;
using SmartCartCarbonFootprintApi.Helpers;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Services;

using Stripe;
using Stripe.Checkout;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;
        private readonly PaymentService _paymentService;
        private readonly Services.InvoiceService _invoiceService;
        private readonly IEmailService _emailService;


        public StripeController(IOptions<StripeSettings> stripeSettings, PaymentService paymentService , Services.InvoiceService invoiceService, IEmailService emailService)
        {
            _stripeSettings = stripeSettings.Value;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
            _emailService = emailService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentRequest request)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = request.Amount, // Amount in cents
                            Currency = request.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.ProductName
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7008/success",
                CancelUrl = "https://localhost:7008/cancel"
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }
        //Stripe sends webhook events when a payment is successful.
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                "your_webhook_secret" // Get this from Stripe Webhooks settings
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                // 1️⃣ Save payment details in the database
                var payment = new Payment
                {
                    TransactionId = session.Id,
                    Amount = (decimal)(session.AmountTotal / 100), // Convert from cents to dollars
                    Currency = session.Currency,
                    Email = session.CustomerDetails.Email,
                    Status = "Paid",
                    PaymentDate = DateTime.UtcNow
                };

                await _paymentService.SavePaymentAsync(payment);

                // 2️⃣ Generate Invoice as PDF
                var pdfData = _invoiceService.GenerateInvoice(payment);

                // 3️⃣ Send Invoice to User via Email
                await _emailService.SendEmailWithAttachment(payment.Email, "Your Invoice", "Please find your invoice attached.", pdfData, "Invoice.pdf");

            }

            return Ok();
        }


    }
}
