using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AKhderApi.Context;
using AKhderApi.DTOs.UserDtos;
using AKhderApi.Models;

namespace AKhderApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public UserService(AppDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<GetUserProfileDto>> GetAllUsersAsync(int page, int pageSize)
        {
            var users = await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetUserProfileDto>>(users);
        }

        public async Task<GetUserProfileDto?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : _mapper.Map<GetUserProfileDto>(user);
        }

        public async Task<GetUserProfileResponseDto> UpdateUserAsync(string id, UpdateUserProfileDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new GetUserProfileResponseDto { Message = $"No user was found with ID: {id}" };
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var existingEmailUser = await _userManager.FindByEmailAsync(updateUserDto.Email);
                if (existingEmailUser is not null)
                {
                    return new GetUserProfileResponseDto { Message = "Email is already registered!" };
                }
                user.Email = updateUserDto.Email;
                user.NormalizedEmail = updateUserDto.Email.ToUpper(); // Normalize manually
            }

            if (!string.IsNullOrEmpty(updateUserDto.UserName) && updateUserDto.UserName != user.UserName)
            {
                var existingUserNameUser = await _userManager.FindByNameAsync(updateUserDto.UserName);
                if (existingUserNameUser is not null)
                {
                    return new GetUserProfileResponseDto { Message = "Username is already registered!" };
                }
                user.UserName = updateUserDto.UserName;
                user.NormalizedUserName = updateUserDto.UserName.ToUpper();
            }

            _mapper.Map(updateUserDto, user);

            // Update using UserManager to ensure Identity behavior is applied
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new GetUserProfileResponseDto { Message = "Error updating profile!" };
            }

            return new GetUserProfileResponseDto
            {
                Message = "Profile updated successfully!",
                User = _mapper.Map<GetUserProfileDto>(user)
            };
        }


        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
