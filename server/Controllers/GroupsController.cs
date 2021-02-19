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
    [Route("api/[controller]")]
    public class GroupsController : OrchestrateController
    {
        public GroupsController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            List<Group> groups;

            if (IsUserAdmin) groups = await DbContext.Groups.AsNoTracking().Include(_ => _.Manager).ToListAsync();
            else
            {
                var user = await DbContext.Users.AsNoTracking()
                    .Include(_ => _.Roles).ThenInclude(_ => _.Group)
                    .Include(_ => _.ManagingGroups)
                    .Include(_ => _.DirectorOfGroups)
                    .FirstOrDefaultAsync(_ => _.Id == RequestingUserId);

                if (user == null) throw new UserNotExistException();

                groups = user.Roles.Select(_ => _.Group)
                    .Concat(user.ManagingGroups)
                    .Concat(user.DirectorOfGroups)
                    .OrderBy(_ => _.Name)
                    .ToList();
            }

            return Ok(ModelMapper.Map<IEnumerable<GroupData>>(groups));
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupInfo(int groupId)
        {
            var group = await DbContext.Groups.AsNoTracking()
                .Include(_ => _.Manager)
                .Include(_ => _.Directors)
                .Include(_ => _.AvailableRoles)
                .Include(_ => _.AssignedRoles).ThenInclude(_ => _.User)
                .FirstAsync(_ => _.Id == groupId);

            return Ok(new FullGroupData()
            {
                Id = group.Id,
                Name = group.Name,
                Manager = ModelMapper.Map<UserData>(group.Manager),
                Directors = ModelMapper.Map<IEnumerable<UserData>>(group.Directors),
                Roles = group.AvailableRoles.GroupJoin(group.AssignedRoles, _ => _.Id, _ => _.RoleId,
                    (role, assignedRoles) => new AssignedRoleData
                    {
                        Id = role.Id,
                        Section = role.Section,
                        Num = role.Num,
                        Members = ModelMapper.Map<IEnumerable<UserData>>(assignedRoles.Select(_ => _.User))
                    })
            });
        }

        #region Administrator Routes

        [HttpPost]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> CreateGroup([Bind("Name,ManagerId")] Group group)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            DbContext.Groups.Add(group);
            await DbContext.SaveChangesAsync();

            var dbGroup = await DbContext.Groups
                .AsNoTracking()
                .Include(_ => _.Manager)
                .FirstAsync(_ => _.Id == group.Id);

            return Ok(ModelMapper.Map<GroupData>(dbGroup));
        }

        [HttpPut("{groupId}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> UpdateGroup([FromRoute] int groupId, [Bind("Name,ManagerId")] Group group)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            if (await DbContext.Groups.AllAsync(_ => _.Id != group.Id)) return NotFound(new { Error = "Group not found" });

            DbContext.Groups.Update(group);
            await DbContext.SaveChangesAsync();

            var dbGroup = await DbContext.Groups
                .AsNoTracking()
                .Include(_ => _.Manager)
                .FirstAsync(_ => _.Id == group.Id);

            return Ok(ModelMapper.Map<GroupData>(dbGroup));
        }

        [HttpDelete("{groupId}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> DeleteGroup([FromRoute] int groupId)
        {
            var group = await DbContext.Groups.FindAsync(groupId);
            if (group == null) return NotFound(new { Error = "Group not found" });

            DbContext.Groups.Remove(group);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
