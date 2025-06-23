using AKhderApi.DTOs;
using AKhderApi.Repositories;
using AKhderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public WeightController(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        [HttpGet("weight")]
        public async Task<IActionResult> Weight()
        {
            var userCart = await _cartService.GetCart();
            return Ok(new { weight = userCart.ActualWeight });
        }

        [HttpPost]
        public async Task<IActionResult> SendWeight([FromBody] WeightDto model)
        {
            var userCart = await _cartService.GetCart();
            userCart.ActualWeight = model.ActualWeight;
            await _unitOfWork.CompleteAsync();
            return Ok();
           
        }
    }
}