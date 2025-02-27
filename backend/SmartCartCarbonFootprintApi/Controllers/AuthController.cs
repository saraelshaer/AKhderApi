using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Azure;
using Microsoft.AspNetCore.Identity;
using System.Net;
using SmartCartCarbonFootprintApi.DTOs.AuthDtos;
using Microsoft.Extensions.Caching.Memory;

namespace SmartCartCarbonFootprintApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly APIResponse response;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<User> userManager, IAuthService authService, IConfiguration config, IMemoryCache cache, IEmailService emailService)
        {
            _userManager = userManager;
            _authService = authService;
            _config = config;
            _cache = cache;
            _emailService = emailService;
            response = new APIResponse();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        //Login
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        private string GenerateJwtToken(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, email)
    };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("signin-google")]
        [AllowAnonymous]
        public IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("GoogleResponse")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Google authentication failed.");
                return BadRequest(response);
            }

            // Extract user information from the claims
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()
                                 ?.Claims.Select(claim => new
                                 {
                                     claim.Type,
                                     claim.Value
                                 });

            var emailClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
            var nameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Name);
            var givenNameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName);
            var surnameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Surname);

            var email = emailClaim.Value;
            var firstName = givenNameClaim?.Value;
            var lastName = surnameClaim?.Value;
            var fullName = nameClaim?.Value;

            if (emailClaim is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Email claim not received from Google.");
                return BadRequest(response);
            }

            var user = await _userManager.FindByEmailAsync(emailClaim.Value);

            if (user is null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName
                };

                if (!string.IsNullOrEmpty(fullName))
                {
                    var names = fullName.Split(' ');
                    firstName = names.FirstOrDefault();
                    lastName = names.Skip(1).FirstOrDefault();
                    user.FirstName = firstName;
                    user.LastName = lastName;
                }

                if (string.IsNullOrEmpty(firstName)) user.FirstName = "First";
                if (string.IsNullOrEmpty(lastName)) user.LastName = "Last";

                var res = await _userManager.CreateAsync(user);

                if (!res.Succeeded)
                {
                    response.Result = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("Could not create user.");
                }
            }

            var token = await _authService.CreateJwtToken(user);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new { token };
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Store OTP mapped to the email (valid for 5 minutes)
            _cache.Set(otp, user.Email, TimeSpan.FromMinutes(5));

            // Send OTP via email
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code",
                $"Your OTP code is: {otp}. It is valid for 5 minutes.");

            return Ok(new { message = "OTP has been sent to your email." });
        }



        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            if (!_cache.TryGetValue(model.Otp, out string email))
            {
                return BadRequest(new { message = "Invalid or expired OTP. Try Again" });
            }

            // OTP is correct, generate a temporary token
            var tempToken = Guid.NewGuid().ToString();

            // Store temporary token mapped to the email (valid for 10 minutes)
            _cache.Set(tempToken, email, TimeSpan.FromMinutes(10));

            // Remove OTP after use (security best practice)
            _cache.Remove(model.Otp);

            return Ok(new { tempToken });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!_cache.TryGetValue(model.TempToken, out string email))
            {
                return BadRequest(new { message = "Invalid or expired session." });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await _userManager.ResetPasswordAsync(user,
                await _userManager.GeneratePasswordResetTokenAsync(user),
                model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Remove temp token after successful reset
            _cache.Remove(model.TempToken);

            return Ok(new { message = "Password reset successfully." });
        }

    }
}
