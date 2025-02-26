using SmartCartCarbonFootprintApi.DTOs.UserDtos;

namespace SmartCartCarbonFootprintApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<GetUserProfileDto>> GetAllUsersAsync(int page, int pageSize);
        Task<GetUserProfileDto?> GetUserByIdAsync(string id);
        Task<GetUserProfileResponseDto> UpdateUserAsync(string id, UpdateUserProfileDto updateUserDto);
        Task<bool> DeleteUserAsync(string id);
    }
}
