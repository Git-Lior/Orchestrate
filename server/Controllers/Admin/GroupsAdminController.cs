using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/groups")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class GroupsAdminController : OrchestrateController
    {
        public GroupsAdminController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var allGroups = await DbContext.Groups.AsNoTracking().Include(_ => _.Manager).ToListAsync();
            return Ok(ModelMapper.Map<IEnumerable<GroupData>>(allGroups));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([Bind("Name,ManagerId")] Group group)
        {
            DbContext.Groups.Add(group);
            await DbContext.SaveChangesAsync();

            return Ok(await GetGroupData(group.Id));
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup([Bind("Name,ManagerId")] Group group)
        {
            var dbGroup = await DbContext.Groups.FindAsync(group.Id);
            if (dbGroup == null) return NotFound(new { Error = "Group not found" });

            DbContext.Groups.Update(group);
            await DbContext.SaveChangesAsync();

            return Ok(await GetGroupData(group.Id));
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int groupId)
        {
            var group = await DbContext.Groups.FindAsync(groupId);
            if (group == null) return NotFound(new { Error = "Group not found" });

            DbContext.Groups.Remove(group);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<GroupData> GetGroupData(int groupId)
        {
            var dbGroup = await DbContext.Groups
                .AsNoTracking()
                .Include(_ => _.Manager)
                .FirstAsync(_ => _.Id == groupId);

            return ModelMapper.Map<GroupData>(dbGroup);
        }
    }
}
