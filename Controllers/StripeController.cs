using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartCartCarbonFootprintApi.DTOs.PaymentDtos;
using SmartCartCarbonFootprintApi.Helpers;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Services;

using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

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
        private readonly StripeService _stripeService;
        private readonly UserManager<User> _userManager;
        private readonly Services.OrderService _orderService;


        public StripeController(IOptions<StripeSettings> stripeSettings, PaymentService paymentService , Services.InvoiceService invoiceService, IEmailService emailService , StripeService stripeService , UserManager<User> userManager , Services.OrderService orderService)
        {
            _stripeSettings = stripeSettings.Value;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
            _emailService = emailService;
            _stripeService = stripeService;
            _userManager = userManager;
            _orderService = orderService;
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
        [AllowAnonymous]
        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess()
        {
            try
            {
                await _orderService.CreateOrder("stripe_user"); // إنشاء الطلب بعد الدفع
                return Ok(new { message = "Payment successful, order created." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("payment-cancel")]
        public IActionResult PaymentCancel()
        {
            return Ok(new { message = "Payment was canceled by the user." });
        }

    }
}
