using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.backend.DTOs.SharedDto;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
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
        public async Task<IActionResult> GetAllFavouriteItems(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });


            var wishlist = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId, new[] { "ProductWishlists" });

            if (wishlist == null  || !wishlist.ProductWishlists.Any())
                return NotFound(new { message = "No favourite products found." });

            var totalProducts = wishlist.ProductWishlists.Count;

            var wishlistItems = wishlist.ProductWishlists.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(w => w.Product).ToList();

            var productsPagination = new PaginationDto<ReadProductDto>
            {
                TotalCount = totalProducts,
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadProductDto>>(wishlistItems)
            };

            return Ok(productsPagination);
        }

        [HttpGet("{productId}/exists")]
        public async Task<IActionResult> IsProductInFavourite(string productId)
        {
            if (!await _unitOfWork.Products.Exists(p => p.Id == productId))
                return NotFound($"No product was found with ID: {productId}");

            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });


            var wishlist = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId, new[] { "ProductWishlists" });

            if (wishlist == null || wishlist?.ProductWishlists == null || !wishlist.ProductWishlists.Any())
                return NotFound(new { isFavourite = false });

            var exists = wishlist.ProductWishlists.Any( pw => pw.ProductId == productId );

            return Ok(new { isFavourite = exists });
        }


        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToFavourite(string productId)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            if (!await _unitOfWork.Products.Exists(p => p.Id == productId))
                return NotFound($"No product was found with ID: {productId}");

            var wishlist = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                };
                await _unitOfWork.Wishlists.AddAsync(wishlist);
                await _unitOfWork.CompleteAsync();
            }

            var wishlistItem = new ProductWishlist()
            {
                ProductId = productId,
                WishlistId = wishlist.Id
            };

            await _unitOfWork.ProductWishlists.AddAsync(wishlistItem);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Product added to favourites successfully." });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromFavourite(string productId)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var wishlist = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId, new[] { "ProductWishlists" });

            var productWishlist = wishlist.ProductWishlists.FirstOrDefault(pw => pw.ProductId == productId);
            if (productWishlist == null)
                return NotFound(new { message = "Product not found in favourites." });

            wishlist.ProductWishlists.Remove(productWishlist);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Product removed from favourites successfully." });
        }


        [HttpDelete("clear")]
        public async Task<IActionResult> ClearFavourites()
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var wishlist = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId, new[] { "ProductWishlists" });

            if (wishlist == null || wishlist?.ProductWishlists == null || !wishlist.ProductWishlists.Any())
                return NotFound(new { message = "No favourite products to clear." });

            wishlist.ProductWishlists.Clear();
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "All favourite products removed successfully." });
        }
    }
}