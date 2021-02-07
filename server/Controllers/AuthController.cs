using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Orchestrate.API.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("auth")]
    public class AuthController : OrchestrateController
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            try
            {
                return Ok(await _userService.Authenticate(info.Email, info.Password));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            try
            {
                return Ok(_userService.GetById(UserId));
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
