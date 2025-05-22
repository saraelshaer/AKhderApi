using AKhderApi.DTOs.WeightCheckDto;
using AKhderApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WeightCheckController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;

        public WeightCheckController(ICartService cartService, IConfiguration configuration)
        {
            _cartService = cartService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CheckWeight([FromBody] WeightCheckDto weightCheckDto)
        {
            var apiKey = Request.Headers["X-API-KEY"];
            if (apiKey != _configuration["ESP32:ApiKey"])
            {
                return Unauthorized(new { message = "Unauthorized device." });
            }

            if (weightCheckDto == null || weightCheckDto.Weight <= 0 || weightCheckDto.CartId <= 0)
                return BadRequest(new { message = "Invalid input provided." });

            var result = await _cartService.CheckWeight(weightCheckDto.CartId, weightCheckDto.Weight);
            if (result)
            {
                return Ok(new { status = "valid", message = "Weight matches" });
            }
            else
            {
                return Ok(new { status = "invalid", message = "Weight mismatch, check for unscanned items" });
            }
        }


    }
}
