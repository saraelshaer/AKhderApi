using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AKhderApi.backend.DTOs.DiscountDto;
using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.Models;
using AKhderApi.Repositories;
using System.Linq.Expressions;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiscountsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts(int pageNumber = 1, int pageSize = 5, bool showExpired = false)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 5 : Math.Min(pageSize, 100);

            Expression<Func<Discount, bool>> filter = d => showExpired || d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Now);

            var discounts = await _unitOfWork.Discounts.GetAllAsync(filter, pageNumber: pageNumber , pageSize: pageSize);

            var discountsPagination = new PaginationDto<ReadDiscountDto>
            {
                TotalCount = await _unitOfWork.Discounts.CountAsync(filter),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadDiscountDto>>(discounts)
            };
            return Ok(discountsPagination);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(int id)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (discount == null) 
                return NotFound($"No discount was found with ID {id} !");

            return Ok(_mapper.Map<ReadDiscountDto>(discount));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromForm]DiscountDto dto)
        {
            var discount = _mapper.Map<Discount>(dto);

            await _unitOfWork.Discounts.AddAsync(discount);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetDiscountById), new {id = discount.Id} , _mapper.Map<ReadDiscountDto>(discount));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id , [FromForm] UpdateDiscountDto dto)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (discount == null)
                return NotFound($"No discount was found with ID {id} !");

            if(dto.Percentage != null)
                discount.Percentage = dto.Percentage.Value;

            if(dto.ExpiryDate != null)
                discount.ExpiryDate = dto.ExpiryDate.Value;

            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<ReadDiscountDto>(discount));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (discount == null)
                return NotFound($"No discount was found with ID {id} !");

            _unitOfWork.Discounts.HardDelete(discount);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
