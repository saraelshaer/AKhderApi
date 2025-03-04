using AutoMapper;
using BlogSystemApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.backend.DTOs.SharedDto;
using SmartCartCarbonFootprintApi.DTOs.CategoryDtos;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Repositories;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(IUnitOfWork unitOfWork , IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
           _unitOfWork = unitOfWork;
           _mapper = mapper;
           _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var categories =await  _unitOfWork.Categories.GetAllAsync
                (
                criteria: c => c.IsActive,
                pageNumber: pageNumber,
                pageSize: pageSize
                ); 
            
            var categoriesPagination = new PaginationDto<GetCategoryDto>
            {
                TotalCount = await _unitOfWork.Categories.CountAsync(c => c.IsActive),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<GetCategoryDto>>(categories)
            };

            return Ok(categoriesPagination);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            var result = _mapper.Map<GetCategoryDto>(category);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryDto dto)
        {
            var relativePath = ImageHelper.SaveImage(dto.ImageFile, "Images", _webHostEnvironment);
            var category = _mapper.Map<Category>(dto);
            category.ImagePath = relativePath;

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetCategoryById), new {id = category.Id} ,_mapper.Map<GetCategoryDto>(category));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound($"No category was found with ID: {id}");

            if(dto.ImageFile != null)
            {
                var relativePath = ImageHelper.SaveImage(dto.ImageFile, "Images", _webHostEnvironment);
                category.ImagePath = relativePath;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var exists = await _unitOfWork.Categories.Exists(c => c.Name == dto.Name && c.Id != id);
                if (exists)
                    return BadRequest($"Name {dto.Name} already exists!");

                category.Name = dto.Name;
            }
           
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<GetCategoryDto>(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound($"No category was found with ID: {id}");

            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
