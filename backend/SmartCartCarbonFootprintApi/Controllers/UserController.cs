using AutoMapper;
using BlogSystemApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCartCarbonFootprintApi.DTOs.CategoryDtos;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
using SmartCartCarbonFootprintApi.DTOs.UserDtos;
using SmartCartCarbonFootprintApi.Helpers;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Repositories;
using SmartCartCarbonFootprintApi.Services;
using SmartCartCarbonFootprintApi.Validators;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UpdateUserProfileDto> _validator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public UserController(IUserService userService, IValidator<UpdateUserProfileDto> validator, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork , IMapper mapper)
        {
            _userService = userService;
            _validator = validator;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsrs(int pageNumber = 1, int pageSize = 10)
        {
            var users = await _unitOfWork.Users.GetAllAsync
                (
                criteria: u => u.IsActive,
                pageNumber: pageNumber,
                pageSize: pageSize
                );
            return Ok(_mapper.Map<IEnumerable<GetUserProfileDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var product = await _unitOfWork.Users.Find(u => u.Id == id);
            return Ok(_mapper.Map<GetUserProfileDto>(product));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserProfileDto updateUserDto)
        {
            var validationResult = await _validator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _unitOfWork.Users.Find(u => u.Id == id);
            if (user == null)
                return NotFound($"No User was found with ID: {id}");

            if (updateUserDto.Imagefile != null)
            {
                var relativePath = ImageHelper.SaveImage(updateUserDto.Imagefile, "Images", _webHostEnvironment);
                user.ImageFileName = relativePath;
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok( result);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _unitOfWork.Users.Find(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");
            user.IsActive = false;
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

    }
}
