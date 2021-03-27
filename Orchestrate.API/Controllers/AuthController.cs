using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.API.Services.Interfaces;
using Orchestrate.Data.Models;
using Orchestrate.Data.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AdminOptions _adminOptions;
        private readonly IUsersRepository _usersRepo;

        public AuthController(IServiceProvider provider,
                              IOptions<AdminOptions> adminOptions,
                              ITokenGenerator tokenGenerator,
                              IUsersRepository repository) : base(provider)
        {
            _adminOptions = adminOptions.Value;
            _tokenGenerator = tokenGenerator;
            _usersRepo = repository;
        }

        [AllowAnonymous]
        [HttpPost("admin")]
        public IActionResult Admin([FromBody] string password)
        {
            if (password != _adminOptions.AdminPassword)
                throw new ArgumentException("Incorrect admin credentials");

            return Ok(_tokenGenerator.GenerateAdminToken());
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload info)
        {
            var user = await _usersRepo.AuthenticateUser(info.Email, info.Password);
            
            var userData = Mapper.Map<LoggedInUserData>(user);
            userData.Token = _tokenGenerator.GenerateUserToken(userData.Id);

            return Ok(userData);
        }

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            try
            {
                var user = await SingleOrError(_usersRepo.FindOne(new UserIdentifier(RequestingUserId)).ProjectTo<UserData>(MapperConfig));

                return Ok(user);
            }
            catch
            {
                throw new UserNotExistException();
            }
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordPayload payload)
        {
            var user = await SingleOrError(_usersRepo.FindOne(new UserIdentifier(RequestingUserId)));
            await _usersRepo.ChangePassword(user, payload.OldPassword, payload.NewPassword);

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
