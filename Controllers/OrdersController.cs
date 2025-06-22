using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.Consts;
using AKhderApi.DTOs.OrderDtos;
using AKhderApi.Models;
using AKhderApi.Repositories;
using AKhderApi.Services;
using AutoMapper;
using BlogSystemApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Security.Claims;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartService = cartService;
        }


        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var userOrders = await _unitOfWork.Orders.GetAllAsync(
                criteria: o => o.UserId == userId, 
                includes: new[] { "ProductOrders.Product" },
                orderBy: o => o.CreatedAt,
                orderByDirection: OrderByDirection.Descending,
                pageNumber: pageNumber,
                pageSize: pageSize);

            if (userOrders== null || !userOrders.Any() )
                return NotFound(new { message = "No orders found." });

            var productsPagination = new PaginationDto<ReadOrderDto>
            {
                TotalCount = await _unitOfWork.Orders.CountAsync(o => o.UserId == userId),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadOrderDto>>(userOrders)
            };

            return Ok(productsPagination);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _unitOfWork.Orders.FindAsync(o => o.Id == orderId , new[] { "ProductOrders.Product" });
            if (order == null) 
                return NotFound(new { message = $"No order was found with ID: {orderId}" });

            var readOrder = _mapper.Map<ReadOrderDto>(order);

            return Ok(readOrder);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] PaymentMethod paymentMethod)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
                return BadRequest(new { message = "Invalid payment method." });

            var (totalPrice, totalCarbonFootprint) = await _cartService.CalculateCartTotal();

            var userCart = await _cartService.GetCart();
            if (userCart == null || !userCart.ProductCarts.Any())
                return NotFound(new { message = "No products found in the cart." });

            var order = new Order
            {
                UserId = userId,
                TotalCarbonFootprint = totalCarbonFootprint,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                TransactionStatus = TransactionStatus.Pending,
                ProductOrders = userCart.ProductCarts.Select(pc => new ProductOrder
                {
                    ProductId = pc.ProductId,
                    Quantity = pc.Quantity
                }).ToList(),

            };

            await _unitOfWork.Orders.AddAsync(order);
            userCart.ProductCarts.Clear();
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id}, _mapper.Map<ReadOrderDto>(order));
        }
    }
}
