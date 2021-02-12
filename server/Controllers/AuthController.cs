using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orchestrate.API.DTOs;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : OrchestrateController
    {   
        private readonly IPasswordProvider _passwordProvider;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AdminOptions _adminOptions;

        public AuthController(IPasswordProvider passwordProvider, IOptions<AdminOptions> adminOptions, ITokenGenerator tokenGenerator, IServiceProvider provider)
            : base(provider)
        {
            _passwordProvider = passwordProvider;
            _adminOptions = adminOptions.Value;
            _tokenGenerator = tokenGenerator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload info)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(_ => _.Email == info.Email);
            if (user == null) return NotFound(new { Error = "User not found" });

            var (success, needsUpgrade) = _passwordProvider.CheckHash(user.PasswordHash, info.Password);

            if (!success) return BadRequest(new { Error = "Incorrect password" });

            if (needsUpgrade)
            {
                user.PasswordHash = _passwordProvider.HashPassword(info.Password);
                await DbContext.SaveChangesAsync();
            }

            var userData = ModelMapper.Map<UserDataWithToken>(user);
            userData.Token = _tokenGenerator.GenerateUserToken(user.Id);

            return Ok(userData);
        }

        [AllowAnonymous]
        [HttpPost("admin")]
        public IActionResult Admin([FromBody] string password)
        {
            if (password != _adminOptions.AdminPassword)
                return BadRequest(new { Error = "Incorrect admin credentials" });

            return Ok(_tokenGenerator.GenerateAdminToken());
        }

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            return Ok(ModelMapper.Map<UserData>(await GetRequestingUser()));
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordPayload payload)
        {
            var user = await GetRequestingUser();

            var (success, _) = _passwordProvider.CheckHash(user.PasswordHash, payload.OldPassword);
            if (!success) return BadRequest(new { Error = "Incorrect Password" });

            user.PasswordHash = _passwordProvider.HashPassword(payload.NewPassword);
            await DbContext.SaveChangesAsync();

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
