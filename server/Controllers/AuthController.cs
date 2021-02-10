using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orchestrate.API.Data;
using Orchestrate.API.DTOs;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : OrchestrateController
    {
        private readonly IMapper _mapper;
        private readonly OrchestrateContext _context;
        private readonly IPasswordProvider _passwordProvider;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AdminOptions _adminOptions;

        public AuthController(IMapper mapper, OrchestrateContext context, IPasswordProvider passwordProvider, IOptions<AdminOptions> adminOptions, ITokenGenerator tokenGenerator)
        {
            _mapper = mapper;
            _context = context;
            _passwordProvider = passwordProvider;
            _adminOptions = adminOptions.Value;
            _tokenGenerator = tokenGenerator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload info)
        {
            var user = await _context.Users.FirstOrDefaultAsync(_ => _.Email == info.Email);
            if (user == null) return NotFound(new { Error = "User not found" });

            var (success, needsUpgrade) = _passwordProvider.CheckHash(user.PasswordHash, info.Password);

            if (!success) return BadRequest(new { Error = "Incorrect password" });

            if (needsUpgrade)
            {
                user.PasswordHash = _passwordProvider.HashPassword(info.Password);
                await _context.SaveChangesAsync();
            }

            var userData = _mapper.Map<UserDataWithToken>(user);
            userData.Token = _tokenGenerator.GenerateUserToken(user.Id);

            return Ok(userData);
        }

        [AllowAnonymous]
        [HttpPost("admin")]
        public IActionResult Admin([FromBody] string password)
        {
            if (password != _adminOptions.AdminPassword)
                return BadRequest(new { Error = "Invalid admin credentials" });

            return Ok(_tokenGenerator.GenerateAdminToken());
        }

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            var user = await _context.Users.FindAsync(RequestingUserId);
            return Ok(_mapper.Map<UserData>(user));
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordPayload payload)
        {
            var user = await _context.Users.FindAsync(RequestingUserId);
            if (user == null) return NotFound(new { Error = "User not found" });

            var (success, _) = _passwordProvider.CheckHash(user.PasswordHash, payload.OldPassword);
            if (!success) return BadRequest(new { Error = "Incorrect Password" });

            user.PasswordHash = _passwordProvider.HashPassword(payload.NewPassword);
            await _context.SaveChangesAsync();

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
