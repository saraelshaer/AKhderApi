using AKhderApi.DTOs.AuthDtos;
using AKhderApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace AKhderApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<JwtSecurityToken> CreateJwtToken(User user);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task LogoutAsync();

    }
}
