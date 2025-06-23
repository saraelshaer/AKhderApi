using AKhderApi.DTOs;
using AKhderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {


        [HttpPost]
        public async Task<IActionResult> SendWeight([FromBody] WeightDto model)
        {
            return Ok(new{ ActualWeight = model.ActualWeight});
        }
    }
}