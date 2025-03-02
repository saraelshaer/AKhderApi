using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
using SmartCartCarbonFootprintApi.DTOs.SharedDto;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Repositories;
using System.Security.Claims;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavouriteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public FavouriteController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllProductts(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found");

            var user = await _unitOfWork.Users.FindAsync( u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var products = user.Wishlist.ProductWishlists.Select(w => w.Product);

            var productsPagination = new PaginationDto<ReadProductDto>
            {
                TotalCount = await _unitOfWork.Products.CountAsync(),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadProductDto>>(products)
            };

            return Ok(productsPagination);
        }
    }
}
