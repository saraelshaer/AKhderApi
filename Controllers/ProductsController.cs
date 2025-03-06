using AutoMapper;
using BlogSystemApi.Consts;
using BlogSystemApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.DTOs.ProductDtos;
using AKhderApi.Models;
using AKhderApi.Repositories;
using AKhderApi.Services;
using System.Linq.Expressions;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly QRCodeService _qrCodeService;
        private readonly IConfiguration _configuration;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment, QRCodeService qrCodeService , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _qrCodeService = qrCodeService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            int pageNumber = 1, int pageSize = 10, 
            int? categoryId = null, int? discountId = null ,
            string orderBy = "CarbonFootprint", bool ascending = true, string? searchQuery = null)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var validOrderByFields = new HashSet<string> { "carbonfootprint", "price", "name", "createddate" };
            orderBy = orderBy.Trim().ToLower();
            if (!validOrderByFields.Contains(orderBy))
            {
                return BadRequest("Invalid orderBy value. Allowed values are { carbonfootprint , price , createddate , name }");
            }

            Expression<Func<Product , bool>> filter = p => p.IsActive &&
              (!categoryId.HasValue || p.CategoryId == categoryId)    &&
              (!discountId.HasValue || p.DiscountId == discountId)    &&
              (string.IsNullOrEmpty(searchQuery) ||
                p.Name.Contains(searchQuery)     ||   
                p.Category.Name.Contains(searchQuery));

            Expression<Func< Product, object>> orderByFunc = orderBy.Trim().ToLower() switch
            {
                "carbonfootprint" => p => p.CarbonFootprint,
                "price" => p => p.Price,
                "name" => p => p.Name,
                "createddate" => p => p.CreatedDate,
                _ => p => p.CarbonFootprint
            };

            var products = await _unitOfWork.Products.GetAllAsync
                (
                criteria: filter,
                includes: new[] { "Category", "Discount" },
                orderBy: orderByFunc,
                orderByDirection: ascending? OrderByDirection.Ascending : OrderByDirection.Descending,
                pageNumber: pageNumber,
                pageSize: pageSize
                );

            var productsPagination = new PaginationDto<ReadProductDto>
            {
                TotalCount = await _unitOfWork.Products.CountAsync(filter),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadProductDto>>(products)
            };

            return Ok(productsPagination);
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

        [HttpGet("GetQRCode/{id}")]
        public async Task<IActionResult> GetQRCode(string id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync<string>(id);
            if (product == null)
                return NotFound($"No product was found with ID: {id}");

            string? baseUrl = _configuration.GetValue<string>("BaseUrl");
            if (string.IsNullOrEmpty(baseUrl))
                return StatusCode(500, "Base URL is not configured.");

            if (!string.IsNullOrEmpty(product.QRCode))
                return Ok(new { QRCodeUrl = $"{baseUrl}{product.QRCode}" });

            string productUrl = baseUrl + $"/api/Products/{id}";
            string qrCodePath = _qrCodeService.GenerateQRCode(productUrl, id);

            product.QRCode = qrCodePath;
            await _unitOfWork.CompleteAsync();

            return Ok(new { QRCodeUrl = $"{baseUrl}{qrCodePath}" });
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

            return CreatedAtAction(nameof(GetProductById), new {id = product.Id},_mapper.Map<ReadProductDto>(product));
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

            return NoContent();
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