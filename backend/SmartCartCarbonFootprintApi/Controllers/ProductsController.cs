using AutoMapper;
using BlogSystemApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
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

        [HttpGet("{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10)
        {
            var products = await _unitOfWork.Products.GetAllAsync
                (
                criteria: p => p.IsActive,
                pageNumber: pageNumber,
                pageSize: pageSize
                );
            return Ok(_mapper.Map<IEnumerable<ReadProductDto>>(products));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _unitOfWork.Products.Find(p => p.Id == id , new[]{"Category", "Discount" });
            return Ok(_mapper.Map<ReadProductDto>(product));
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
    }
}