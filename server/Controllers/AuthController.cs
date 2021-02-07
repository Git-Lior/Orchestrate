using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("auth")]
    public class AuthController : OrchestrateController
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AuthController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
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

        [AllowAnonymous]
        [HttpPost("admin")]
        public IActionResult Admin([FromBody] string password)
        {
            try
            {
                return Ok(_adminService.Authenticate(password));
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
