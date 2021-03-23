using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/groups")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class GroupsAdminController : EntityApiControllerBase<Group>
    {
        [FromRoute]
        public int GroupId { get; set; }

        protected override string EntityName => "Group";
        protected override IQueryable<Group> MatchingEntityQuery(IQueryable<Group> query)
            => query.Where(_ => _.Id == GroupId);

        public GroupsAdminController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            return Ok(await DbContext.Groups.AsNoTracking().ProjectTo<GroupData>(MapperConfig).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupPayload payload)
        {
            DbContext.Groups.Add(ModelMapper.Map<Group>(payload));
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupPayload payload)
        {
            var group = await GetMatchingEntity(DbContext.Groups);

            ModelMapper.Map(payload, group);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup()
        {
            var group = await GetMatchingEntity(DbContext.Groups);

            DbContext.Groups.Remove(group);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
