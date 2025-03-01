using AutoMapper;
using BlogSystemApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
using SmartCartCarbonFootprintApi.DTOs.SharedDto;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Repositories;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) 
                 pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            var products = await _unitOfWork.Products.GetAllAsync
                (
                criteria: p => p.IsActive,
                includes: new[] { "Category", "Discount" },
                pageNumber: pageNumber,
                pageSize: pageSize
                );

            var ProductsPagination = new PaginationDto<ReadProductDto>
            {
                TotalCount = await _unitOfWork.Products.Count(p => p.IsActive),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadProductDto>>(products)
            };
            return Ok(ProductsPagination);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id , new[]{"Category", "Discount" });
            if (product == null)
                return NotFound($"No product was found with ID: {id}");

            var productTDto = _mapper.Map<ReadProductDto>(product);
            if (product.DiscountId != null)
            {
                productTDto.DiscountedPrice = product.Price * (1 - product.Discount.Percentage / 100);
            }
            return Ok(productTDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var exists = await _unitOfWork.Categories.Exists(c => c.Id == dto.CategoryId);
            if (!exists)
                return NotFound($"No category was found with ID: {dto.CategoryId}");

            if (dto.DiscountId != null)
            {
                if(!await _unitOfWork.Discounts.Exists(d => d.Id == dto.DiscountId))
                    return NotFound($"No Discount was found with ID: {dto.DiscountId}");

            }
            var relativePath = ImageHelper.SaveImage(dto.ImageFile, "Images", _webHostEnvironment);
            var product = _mapper.Map<Product>(dto);
            product.ImagePath = relativePath;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<ReadProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id , [FromForm] UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id);
            if (product == null)
                return NotFound($"No product was found with ID: {id}");

            if (dto.CategoryId != null && !await _unitOfWork.Categories.Exists(c => c.Id == dto.CategoryId))
            {
                return NotFound($"No category was found with ID: {dto.CategoryId}");
            }
   

            if (dto.DiscountId != null && !await _unitOfWork.Discounts.Exists(d => d.Id == dto.DiscountId))
            {
                return NotFound($"No Discount was found with ID: {dto.DiscountId}");
            }

            var result = _mapper.Map(dto, product);
            if (dto.ImageFile != null)
            {
                var relativePath = ImageHelper.SaveImage(dto.ImageFile, "Images", _webHostEnvironment);
                result.ImagePath = relativePath;
            }

            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<ReadProductDto>(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound($"No product was found with ID: {id}");

            _unitOfWork.Products.SoftDelete(product);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}