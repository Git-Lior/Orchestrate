using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.Data.Repositories;
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
    public class GroupManagerController : ApiControllerBase
    {
        private readonly RolesRepository _rolesRepo;
        private readonly GroupsRepository _groupsRepo;

        [FromRoute]
        public int GroupId { get; set; }

        public GroupIdentifier EntityId => new GroupIdentifier(GroupId);

        public GroupManagerController(IServiceProvider provider, GroupsRepository repository, RolesRepository rolesRepo) : base(provider)
        {
            _groupsRepo = repository;
            _rolesRepo = rolesRepo;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await Repository.Get<User>().NoTrackedEntities.ProjectTo<UserData>(MapperConfig).ToListAsync());
        }

        [HttpGet("updates")]
        public async Task<IActionResult> GetUpdates()
        {
            var timeBound = DateTime.Today.AddDays(-7);

            var compositions = await Repository.Get<Composition>().NoTrackedEntities
                .Where(c => c.GroupId == GroupId && c.CreatedAt > timeBound)
                .ProjectTo<CompositionUpdateData>(MapperConfig)
                .ToListAsync();

            var concerts = await Repository.Get<Concert>().NoTrackedEntities
                .Include(c => c.Attendances.Where(_ => _.UpdatedAt > timeBound))
                .Where(c => c.Attendances.Count > 0)
                .ToListAsync();

            IEnumerable<dynamic> concertDatas = concerts
                .Select(c => new ConcertUpdateData
                {
                    Date = Mapper.Map<long>(c.Attendances.Max(_ => _.UpdatedAt)),
                    Concert = Mapper.Map<BasicConcertData>(c),
                    Attendance = c.Attendances.Count
                });

            return Ok(concertDatas.Concat(compositions).OrderByDescending(_ => _.Date));
        }

        [HttpPost("directors")]
        public async Task<IActionResult> AddDirector([FromBody] int directorId)
        {
            var director = await SingleOrError(Repository.Get<User>().FindOne(new UserIdentifier(directorId)));
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Directors));

            await _groupsRepo.AddDirector(group, director);

            return Ok();
        }

        [HttpDelete("directors/{directorId}")]
        public async Task<IActionResult> RemoveDirector([FromRoute] int directorId)
        {
            var director = await SingleOrError(Repository.Get<User>().FindOne(new UserIdentifier(directorId)));
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Directors));

            await _groupsRepo.RemoveDirector(group, director);

            return Ok();
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromBody] RolePayload payload)
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Roles));
            var role = await _rolesRepo.GetOrCreate(payload.Section, payload.Num);

            var groupRole = await _groupsRepo.AddRole(group, role);

            return Ok(Mapper.Map<GroupRoleData>(groupRole));
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> RemoveRole([FromRoute] int roleId)
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Roles));

            await _groupsRepo.RemoveRole(group, roleId);

            return Ok();
        }

        [HttpPost("roles/{roleId}/members")]
        public async Task<IActionResult> AddMember([FromRoute] int roleId, [FromBody] int memberId)
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Roles).ThenInclude(_ => _.Members));

            var user = await SingleOrError(Repository.Get<User>().FindOne(new UserIdentifier(memberId)));

            await _groupsRepo.AddMember(group, roleId, user);

            return Ok();
        }

        [HttpDelete("roles/{roleId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] int roleId, [FromRoute] int memberId)
        {
            var group = await SingleOrError(_groupsRepo.FindOne(EntityId).Include(_ => _.Roles).ThenInclude(_ => _.Members));

            var user = await SingleOrError(Repository.Get<User>().FindOne(new UserIdentifier(memberId)));

            await _groupsRepo.RemoveMember(group, roleId, user);

            return Ok();
        }
    }
}
