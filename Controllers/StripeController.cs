using AKhderApi.Models;
using AKhderApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.Services;
using System.Security.Claims;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StripeController : ControllerBase
    {
        private readonly StripeService _stripeService;
        private readonly UserManager<User> _userManager;
        private readonly Services.OrderService _orderService;
        private readonly IEmailService _emailService;


        public StripeController(StripeService stripeService, UserManager<User> userManager, Services.OrderService orderService, IEmailService emailService)
        {
            _stripeService = stripeService;
            _userManager = userManager;
            _orderService = orderService;
            _emailService = emailService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            try
            {
                string sessionUrl = await _stripeService.CreateCheckoutSession(userId);
                return Ok(new { url = sessionUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        private string GenerateInvoiceEmailBody(dynamic invoice)
        {
            var emailBody = new System.Text.StringBuilder();
            emailBody.AppendLine($"📄 Invoice for Order #{invoice.OrderId}");
            emailBody.AppendLine($"🕒 Date: {invoice.CreatedAt}");
            emailBody.AppendLine($"💳 Payment Method: {invoice.PaymentMethod}");
            emailBody.AppendLine($"🔵 Status: {invoice.TransactionStatus}");
            emailBody.AppendLine("\n🛒 Products:");

            foreach (var product in invoice.Products)
            {
                emailBody.AppendLine($"- {product.ProductName} (x{product.Quantity}) - ${product.Total:F2}");
            }

            emailBody.AppendLine("\n💰 Total Price: $" + invoice.TotalPrice);
            emailBody.AppendLine("🌍 Carbon Footprint: " + invoice.TotalCarbonFootprint + " kg CO₂");

            emailBody.AppendLine("\nThank you for shopping with us! 😊");
            return emailBody.ToString();
        }


        [AllowAnonymous]
        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is missing." });

            try
            {
                var order = await _orderService.CreateOrder(userId);

                var invoiceDetails = new
                {
                    OrderId = order.Id,
                    CreatedAt = order.CreatedAt,
                    TotalPrice = order.TotalPrice,
                    TotalCarbonFootprint = order.TotalCarbonFootprint,
                    PaymentMethod = order.PaymentMethod.ToString(),
                    TransactionStatus = order.TransactionStatus.ToString(),
                    Products = order.ProductOrders.Select(po => new
                    {
                        ProductName = po.Product.Name,
                        Quantity = po.Quantity,
                        UnitPrice = po.Product.Price,
                        Total = po.Quantity * po.Product.Price
                    }).ToList()
                };

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string emailSubject = $"Invoice for Order #{order.Id}";
                    string emailBody = GenerateInvoiceEmailBody(invoiceDetails);
                    await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                }

                return Ok(new
                {
                    message = "Payment successful.Invoice has been sent to your email.",
                    invoice = invoiceDetails
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("payment-cancel")]
        public async Task<IActionResult> PaymentCancel([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is missing." });

            try
            {
                await _orderService.UpdateOrderStatus(userId, AKhderApi.Consts.TransactionStatus.Cancelled);
                return Ok(new { message = "Payment was canceled" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
