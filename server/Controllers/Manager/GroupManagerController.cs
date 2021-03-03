using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Manager
{
    [Route("api/groups/{groupId}")]
    [Authorize(Policy = GroupRolesPolicy.ManagerOnly)]
    public class GroupManagerController : OrchestrateController
    {
        public GroupManagerController(IServiceProvider provider) : base(provider) { }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await DbContext.Users.AsNoTracking().ProjectTo<UserData>(MapperConfig).ToListAsync());
        }

        [HttpPost("directors")]
        public async Task<IActionResult> AddDirector([FromRoute] int groupId, [FromBody] int directorId)
        {
            var director = await DbContext.Users.FindAsync(directorId);
            if (director == null) throw new ArgumentException("User not found");

            var group = await DbContext.Groups.Include(_ => _.Directors).FirstAsync(_ => _.Id == groupId);

            group.Directors.Add(director);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("directors/{directorId}")]
        public async Task<IActionResult> RemoveDirector([FromRoute] int groupId, [FromRoute] int directorId)
        {
            var director = await DbContext.Users.FindAsync(directorId);

            if (director == null) throw new ArgumentException("User not found");

            var group = await DbContext.Groups.Include(_ => _.Directors).FirstAsync(_ => _.Id == groupId);

            group.Directors.Remove(director);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromRoute] int groupId, [FromBody] RolePayload role)
        {
            var group = await DbContext.Groups.Include(_ => _.Roles).FirstAsync(_ => _.Id == groupId);

            var dbRole = await DbContext.Roles.FirstOrDefaultAsync(_ => _.Section == role.Section && _.Num == role.Num);

            if (group.Roles.Contains(dbRole))
                throw new ArgumentException("Role already exists in group");

            if (dbRole == null)
            {
                dbRole = ModelMapper.Map<Role>(role);
                DbContext.Roles.Add(dbRole);
                await DbContext.SaveChangesAsync();
            }

            group.Roles.Add(dbRole);
            await DbContext.SaveChangesAsync();

            return Ok(new GroupRoleData
            {
                Id = dbRole.Id,
                Section = dbRole.Section,
                Num = dbRole.Num,
                Members = new List<UserData>()
            });
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> RemoveRole([FromRoute] int groupId, [FromRoute] int roleId)
        {
            var group = await DbContext.Groups
                .Include(_ => _.Roles)
                .Include(_ => _.Members.Where(m => m.RoleId == roleId))
                .FirstAsync(_ => _.Id == groupId);

            var dbRole = group.Roles.FirstOrDefault(r => r.Id == roleId);

            if (dbRole == null) throw new ArgumentException("Role doesn't exist");

            group.Roles.Remove(dbRole);
            DbContext.RemoveRange(group.Members);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("roles/{roleId}/members")]
        public async Task<IActionResult> AddMember([FromRoute] int groupId, [FromRoute] int roleId, [FromBody] int memberId)
        {
            var dbUser = await DbContext.Users.FindAsync(memberId);
            if (dbUser == null) throw new ArgumentException("User doesn't exist");

            var group = await DbContext.Groups
                .Include(_ => _.Roles)
                .Include(_ => _.Members)
                .FirstAsync(_ => _.Id == groupId);

            var role = group.Roles.FirstOrDefault(_ => _.Id == roleId);
            if (role == null) throw new ArgumentException("Role doesn't exist");

            group.Members.Add(new GroupMember { Group = group, Role = role, User = dbUser });
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("roles/{roleId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] int groupId, [FromRoute] int roleId, [FromRoute] int memberId)
        {
            var group = await DbContext.Groups.Include(_ => _.Members).FirstAsync(_ => _.Id == groupId);

            var member = group.Members.FirstOrDefault(_ => _.UserId == memberId && _.RoleId == roleId);
            if (member == null) throw new ArgumentException("Member doesn't exist in this role");

            group.Members.Remove(member);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
