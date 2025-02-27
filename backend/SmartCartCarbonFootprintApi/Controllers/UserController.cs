using AutoMapper;
using BlogSystemApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;



        public UserController(IUserService userService, IValidator<UpdateUserProfileDto> validator, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork , IMapper mapper , UserManager<User> userManager)
        {
            _userService = userService;
            _validator = validator;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
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
        [HttpPost("change-password")]
        //[Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate new password confirmation
            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest(new { message = "New password and confirmation do not match." });

            // Get the logged-in user name from JWT token
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier); ;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var user = await _unitOfWork.Users.Find(u => u.UserName == username);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Attempt to change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Password change failed.", errors });
            }

            return Ok(new { message = "Password changed successfully." });
        }

    }
}
