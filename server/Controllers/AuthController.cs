using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : OrchestrateController
    {
        private readonly IUserService _userService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AdminOptions _adminOptions;

        public AuthController(IUserService userService, IOptions<AdminOptions> adminOptions, ITokenGenerator tokenGenerator)
        {
            _userService = userService;
            _adminOptions = adminOptions.Value;
            _tokenGenerator = tokenGenerator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload info)
        {
            return Ok(await _userService.Authenticate(info.Email, info.Password));
        }

        [AllowAnonymous]
        [HttpPost("admin")]
        public IActionResult Admin([FromBody] string password)
        {
            if (password != _adminOptions.AdminPassword)
                throw new ArgumentException("Invalid admin credentials");

            return Ok(_tokenGenerator.GenerateAdminToken());
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return Ok(_userService.GetById(UserId));
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordPayload payload)
        {
            await _userService.ChangePassword(UserId, payload.OldPassword, payload.NewPassword);
            return Ok();
        }
    }

    public class LoginPayload
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordPayload
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
