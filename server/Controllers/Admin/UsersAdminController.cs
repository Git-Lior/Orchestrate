using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/users")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class UsersAdminController : OrchestrateController
    {
        private readonly IPasswordProvider _passwordProvider;

        public UsersAdminController(IPasswordProvider passwordProvider, IServiceProvider provider) : base(provider)
        {
            _passwordProvider = passwordProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] int groupId)
        {
            if (groupId <= 0)
                return Ok(ModelMapper.Map<IEnumerable<FullUserData>>(await DbContext.Users.AsNoTracking().ToListAsync()));

            var group = await DbContext.Groups.AsNoTracking()
                .Include(_ => _.Manager)
                .Include(_ => _.Directors)
                .Include(_ => _.Members).ThenInclude(_ => _.User)
                .FirstOrDefaultAsync(_ => _.Id == groupId);

            var users = new List<User> { group.Manager }
                .Concat(group.Directors)
                .Concat(group.Members.Select(_ => _.User))
                .Distinct();

            return Ok(ModelMapper.Map<IEnumerable<FullUserData>>(users));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserPayload payload)
        {
            var user = ModelMapper.Map<User>(payload);

            string password = _passwordProvider.GenerateTemporaryPassword(16);
            user.PasswordHash = _passwordProvider.HashPassword(password);
            user.IsPasswordTemporary = true;

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var userData = ModelMapper.Map<CreatedUserData>(user);
            userData.TemporaryPassword = password;

            return Ok(userData);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserPayload payload)
        {
            var dbUser = await DbContext.Users.FindAsync(id);
            if (dbUser == null) return NotFound(new { Error = "User not found" });

            ModelMapper.Map(payload, dbUser);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<UserData>(dbUser));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (await DbContext.Groups.AsNoTracking().AnyAsync(_ => _.ManagerId == id))
                return BadRequest(new { Error = "Cannot delete a user that manages a group, change the group's manager first" });

            var user = await DbContext.Users.FindAsync(id);
            if (user == null) return NotFound(new { Error = "User not found" });

            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
