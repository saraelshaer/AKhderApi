using AKhderApi.DTOs;
using AKhderApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private readonly ICartService _cartService;
        public WeightController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckWeight([FromBody] WeightDto model)
        {
            var expectedWeight = await _cartService.GetTotalWeight();
            if (expectedWeight == 0)
            {
                return NotFound(new { message = "No products found in the cart." });
            }

            if (Math.Abs(model.ActualWeight - expectedWeight) < 0.5m) 
            {
                return Ok(new { message = "Weight matches the expected total weight." });
            }
            else
            {
                return BadRequest(new { message = $"Weight mismatch: Expected {expectedWeight} kg, but got {model.ActualWeight} kg." });
            }
        }
    }
}