using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.Data.Repositories.Interfaces;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/users")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class UsersAdminController : ApiControllerBase
    {
        protected IUsersRepository _usersRepo { get; }

        [FromRoute]
        public int UserId { get; set; }

        public UserIdentifier EntityId => new UserIdentifier(UserId);

        public UsersAdminController(IServiceProvider provider, IUsersRepository repository) : base(provider)
        {
            _usersRepo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] int groupId)
        {
            return Ok(await _usersRepo.GetUsersInGroup(groupId).ProjectTo<FullUserData>(MapperConfig).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserPayload payload)
        {
            (var password, var completePayload) = _usersRepo.GenerateNewUserPayload(payload);

            var userData = await _usersRepo.Create<CreatedUserData>(completePayload);
            userData.TemporaryPassword = password;

            return Ok(userData);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserPayload payload)
        {
            var user = await SingleOrError(_usersRepo.FindOne(EntityId));
            return Ok(await _usersRepo.Update<UserData>(user, payload));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await SingleOrError(_usersRepo.FindOne(EntityId));
            await _usersRepo.Delete(user);
            return Ok();
        }
    }
}
