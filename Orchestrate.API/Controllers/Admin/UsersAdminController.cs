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
using System.Collections.Generic;

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

        [HttpGet, ProducesOk(typeof(IEnumerable<FullUserData>))]
        public async Task<IActionResult> Users([FromQuery] int groupId)
        {
            return Ok(await _usersRepo.GetUsersInGroup(groupId).ProjectTo<FullUserData>(MapperConfig).ToListAsync());
        }

        [HttpPost, ProducesOk(typeof(CreatedUserData))]
        public async Task<IActionResult> CreateUser([FromBody] UserPayload payload)
        {
            (var temporaryPassword, var user) = await _usersRepo.CreateNewUser(payload);

            var userData = Mapper.Map<CreatedUserData>(user);
            userData.TemporaryPassword = temporaryPassword;

            return Ok(userData);
        }

        [HttpPut("{userId}"), ProducesOk(typeof(UserData))]
        public async Task<IActionResult> UpdateUser([FromBody] UserPayload payload)
        {
            var user = await SingleOrError(_usersRepo.FindOne(EntityId));
            var result = await _usersRepo.Update(user, payload);

            return Ok(Mapper.Map<UserData>(result));
        }

        [HttpDelete("{userId}"), ProducesOk]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await SingleOrError(_usersRepo.FindOne(EntityId));
            await _usersRepo.Delete(user);
            return Ok();
        }
    }
}
