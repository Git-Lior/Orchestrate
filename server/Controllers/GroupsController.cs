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
        public async Task<IActionResult> Groups()
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
                    .ToList();
            }

            return Ok(ModelMapper.Map(groups, new List<GroupData>()));
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

        [HttpPut("{id}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> UpdateGroup([FromRoute] int id, [Bind("Name,ManagerId")] Group group)
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

        [HttpDelete("{id}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            var group = await DbContext.Groups.FindAsync(id);
            if (group == null) return NotFound(new { Error = "Group not found" });

            DbContext.Groups.Remove(group);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
