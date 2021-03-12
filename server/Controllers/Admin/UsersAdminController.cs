using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/users")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class UsersAdminController : OrchestrateController<User>
    {
        private readonly IPasswordProvider _passwordProvider;

        [FromRoute]
        public int UserId { get; set; }

        protected override IQueryable<User> MatchingEntityQuery(IQueryable<User> query)
            => query.Where(_ => _.Id == UserId);

        public UsersAdminController(IPasswordProvider passwordProvider, IServiceProvider provider) : base(provider)
        {
            _passwordProvider = passwordProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] int groupId)
        {
            var usersQuery = DbContext.Users.AsNoTracking();

            if (groupId > 0)
                usersQuery = usersQuery.Where(u =>
                       u.ManagingGroups.Any(g => g.Id == groupId)
                    || u.DirectorOfGroups.Any(g => g.Id == groupId)
                    || u.MemberOfGroups.Any(g => g.GroupId == groupId));


            return Ok(await usersQuery.ProjectTo<FullUserData>(MapperConfig).ToListAsync());
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

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserPayload payload)
        {
            var dbUser = await GetMatchingEntity(DbContext.Users);
            if (dbUser == null) return NotFound(new { Error = "User not found" });

            ModelMapper.Map(payload, dbUser);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<UserData>(dbUser));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser()
        {
            if (await DbContext.Groups.AsNoTracking().AnyAsync(_ => _.ManagerId == UserId))
                return BadRequest(new { Error = "Cannot delete a user that manages a group, change the group's manager first" });

            var user = await GetMatchingEntity(DbContext.Users);
            if (user == null) return NotFound(new { Error = "User not found" });

            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
