using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Admin
{
    [Route("api/admin/groups")]
    [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
    public class GroupsAdminController : ApiControllerBase
    {
        private readonly IEntityRepository<Group> _groupsRepo;

        [FromRoute]
        public int GroupId { get; set; }

        public GroupIdentifier EntityId => new GroupIdentifier(GroupId);

        public GroupsAdminController(IServiceProvider provider) : base(provider)
        {
            _groupsRepo = Repository.Get<Group>();
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            return Ok(await _groupsRepo.NoTrackedEntities.ProjectTo<GroupData>(MapperConfig).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupPayload payload)
        {
            await _groupsRepo.Create(payload);
            return Ok();
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupPayload payload)
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId));
            await _groupsRepo.Update(group, payload);
            return Ok();
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup()
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId));
            await _groupsRepo.Delete(group);
            return Ok();
        }
    }
}
