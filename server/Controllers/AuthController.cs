using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Orchestrate.API.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SigningCredentials _signingCredentials;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            var key = System.Text.Encoding.UTF8.GetBytes(config.GetValue<string>("JwtSecret"));
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            try
            {
                var user = await _userService.Authenticate(info.Email, info.Password);
                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.UserId.ToString()) }),
                    SigningCredentials = _signingCredentials
                });

                return Ok(new
                {
                    Id = user.UserId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Token = tokenHandler.WriteToken(token)
                });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }

    public class LoginInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
