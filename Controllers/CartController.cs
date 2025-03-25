using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.DTOs.CartDtos;
using AKhderApi.Models;
using AKhderApi.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AKhderApi.Services;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CartService _cartService;

        public CartController(IUnitOfWork unitOfWork, IMapper mapper, CartService cartService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts.Product" });

            if (userCart == null || !userCart.ProductCarts.Any())
                return NotFound(new { message = "No products found in the cart." });

            var totalProducts = userCart.ProductCarts.Count;

            var CartItems = userCart.ProductCarts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var productsPagination = new PaginationDto<CartItemDto>
            {
                TotalCount = totalProducts,
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<CartItemDto>>(CartItems)
            };

            return Ok(productsPagination);
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetCartTotal()
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts.Product" });

            if (userCart == null || !userCart.ProductCarts.Any())
                return NotFound(new { message = "No products found in the cart." });

            var (totalPrice, totalCarbonFootprint) = await _cartService.CalculateCartTotal(userId);

            return Ok(new { totalPrice , totalCarbonFootprint });
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var product = await _unitOfWork.Products.FindAsync(p => p.Id == productId);

            if (product == null)
                return NotFound(new { message = $"No product was found with ID: {productId}" });

            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId , new[] { "ProductCarts" });

            if (userCart == null)
            {
                userCart = new Cart
                {
                    UserId = userId,
                };
                await _unitOfWork.Carts.AddAsync(userCart);
                await _unitOfWork.CompleteAsync();
            }

            quantity = Math.Max(quantity, 1);

            var cartItem = userCart.ProductCarts.FirstOrDefault(pc => pc.ProductId == productId);

            var newQuantity = cartItem != null ? cartItem.Quantity + quantity : quantity;

            if (newQuantity > product.StockQuantity)
                return BadRequest(new { message = $"Only {product.StockQuantity} items available in stock." });

            if (cartItem != null) 
            {
                cartItem.Quantity = newQuantity;
            }
            else
            {
                cartItem = new ProductCart
                {
                    ProductId = productId,
                    CartId = userCart.Id,
                    Quantity = newQuantity
                };
                await _unitOfWork.ProductCarts.AddAsync(cartItem);
            }
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Product added to cart successfully." });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(string productId, int quantity = 1)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var product = await _unitOfWork.Products.FindAsync(p => p.Id == productId);

            if (product == null)
                return NotFound(new { message = $"No product was found with ID: {productId}" });

            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts" });
            if (userCart == null || !userCart.ProductCarts.Any())
                return NotFound(new { message = "No items in the cart." });

            var cartItem = userCart.ProductCarts.FirstOrDefault(pw => pw.ProductId == productId);
            if (cartItem == null)
                return NotFound(new { message = "Product not found in cart." });

            quantity = Math.Max(quantity, 1);

            cartItem.Quantity -= quantity;

            if(cartItem.Quantity <= 0)
               userCart.ProductCarts.Remove(cartItem);

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Product removed from cart successfully." });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts" });

            if (userCart?.ProductCarts == null || !userCart.ProductCarts.Any())
                return NotFound(new { message = "No products to clear." });

            userCart.ProductCarts.Clear();
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "All products removed successfully." });
        }
    }
}
