using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AKhderApi.Repositories;
using System.Security.Claims;
using AKhderApi.DTOs.ReviewDtos;
using AKhderApi.Models;
using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.DTOs.CartDtos;
using BlogSystemApi.Consts;
using System.Linq.Expressions;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{productId}/reviews")]
        public async Task<IActionResult> GetAllReviews(string productId , int pageNumber = 1, int pageSize = 10,int? rateFilter = null)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);
            if (rateFilter < 1) 
                rateFilter = 1;
            if (rateFilter > 5)
                rateFilter = 5;

            if (!await _unitOfWork.Products.Exists(p => p.Id == productId))
                return NotFound(new { message = $"No product was found with ID: {productId}" });

            Expression<Func<Review, bool>> filter = r => r.ProductId == productId &&
              (!rateFilter.HasValue || r.Rating == rateFilter);

            var totalCount = await _unitOfWork.Reviews.CountAsync(filter);
            if (totalCount == 0) 
                 return NotFound(new { message = "No reviews exist." });

            var reviews = await _unitOfWork.Reviews.GetAllAsync(
                criteria: filter, 
                orderBy: r => r.ReviewDate,
                orderByDirection: OrderByDirection.Descending,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var productsPagination = new PaginationDto<ReadReviewDto>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadReviewDto>>(reviews)
            };
            return Ok(productsPagination);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddOrUpdateReview(string productId , ReviewDto reviewDto)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            if (!await _unitOfWork.Products.Exists(p => p.Id == productId))
                return NotFound(new { message = $"No product was found with ID: {productId}" });

            var existingReview = await _unitOfWork.Reviews.FindAsync(r => r.UserId == userId && r.ProductId == productId);

            if (existingReview != null)
            {
                existingReview.Rating = reviewDto.Rating;
                existingReview.Comment = reviewDto.Comment;
                existingReview.ReviewDate = DateTime.Now;
            }
            else
            {
                var review = _mapper.Map<Review>(reviewDto);
                review.UserId = userId;
                review.ProductId = productId;

                await _unitOfWork.Reviews.AddAsync(review);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Review added/updated successfully." });
        }


    }
}
