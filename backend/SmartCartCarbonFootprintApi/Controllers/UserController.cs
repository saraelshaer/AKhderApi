using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.Repositories;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;


    }
}
