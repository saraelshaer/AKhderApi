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
        private readonly AuthService _authService1;

        public AuthController(UserManager<User> userManager, IAuthService authService, IConfiguration config, AuthService authService1)
        {
            _userManager = userManager;
            _authService = authService;
            _config = config;
            response = new APIResponse();
            _authService1 = authService1;
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

        [Authorize(Roles ="Admin")]
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

            var token = await _authService1.CreateJwtToken(user);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new { token };
            return Ok(response);
        }





    }
}
