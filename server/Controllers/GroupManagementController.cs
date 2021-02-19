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

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}")]
    [Authorize(Policy = GroupRolesPolicy.ManagerOnly)]
    public class GroupManagementController : OrchestrateController
    {
        public GroupManagementController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpPost("directors")]
        public async Task<IActionResult> AddDirector([FromRoute] int groupId, [FromBody] int directorId)
        {
            var director = await DbContext.Users
                .Include(_ => _.DirectorOfGroups)
                .FirstOrDefaultAsync(_ => _.Id == directorId);

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
        public async Task<IActionResult> AddRole([FromRoute] int groupId, [Bind("Section,Num")] Role role)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            var group = await DbContext.Groups.Include(_ => _.AvailableRoles).FirstAsync(_ => _.Id == groupId);

            if (group.AvailableRoles.Any(_ => _.Section == role.Section && _.Num == role.Num))
                throw new ArgumentException("Role already exists in group");

            var dbRole = await DbContext.Roles.FirstOrDefaultAsync(_ => _.Section == role.Section && _.Num == role.Num);

            if (dbRole == null)
            {
                DbContext.Roles.Add(role);
                await DbContext.SaveChangesAsync();
                dbRole = role;
            }

            group.AvailableRoles.Add(dbRole);
            await DbContext.SaveChangesAsync();

            return Ok(new AssignedRoleData
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
            var dbRole = await DbContext.Roles.FindAsync(roleId);
            if (dbRole == null) throw new ArgumentException("Role doesn't exist");

            var group = await DbContext.Groups
                .Include(_ => _.AvailableRoles)
                .Include(_ => _.AssignedRoles)
                .FirstAsync(_ => _.Id == groupId);

            group.AvailableRoles.Remove(dbRole);
            DbContext.RemoveRange(group.AssignedRoles.Where(_ => _.RoleId == roleId));
            
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("roles/{roleId}/members")]
        public async Task<IActionResult> AddMember([FromRoute] int groupId, [FromRoute] int roleId, [FromBody] int memberId)
        {
            var dbUser = await DbContext.Users.FindAsync(memberId);
            if (dbUser == null) throw new ArgumentException("User doesn't exist");

            var dbRole = await DbContext.Roles.FindAsync(roleId);
            if (dbRole == null) throw new ArgumentException("Role doesn't exist");

            var group = await DbContext.Groups.Include(_ => _.AssignedRoles).FirstAsync(_ => _.Id == groupId);

            group.AssignedRoles.Add(new AssignedRole
            {
                Group = group,
                Role = dbRole,
                User = dbUser
            });

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("roles/{roleId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] int groupId, [FromRoute] int roleId, [FromRoute] int memberId)
        {
            var group = await DbContext.Groups.Include(_ => _.AssignedRoles).FirstAsync(_ => _.Id == groupId);

            var assignedRole = group.AssignedRoles.FirstOrDefault(_ => _.RoleId == roleId && _.UserId == memberId);
            if (assignedRole == null) throw new ArgumentException("Member doesn't exist in role");

            group.AssignedRoles.Remove(assignedRole);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
