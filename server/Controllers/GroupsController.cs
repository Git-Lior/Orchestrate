using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
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
            if (await DbContext.Users.AllAsync(_ => _.Id != RequestingUserId)) throw new UserNotExistException();

            return Ok(await DbContext.Groups.Where(_ => _.ManagerId == RequestingUserId)
                .Union(DbContext.Groups.Where(_ => _.Directors.Any(_ => _.Id == RequestingUserId)))
                .Union(DbContext.Groups.Where(_ => _.Members.Any(_ => _.UserId == RequestingUserId)))
                .OrderBy(_ => _.Name)
                .ProjectTo<GroupData>(MapperConfig)
                .ToListAsync());
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupInfo(int groupId)
        {
            var group = await DbContext.Groups.AsNoTracking()
                .Include(_ => _.Manager)
                .Include(_ => _.Directors)
                .Include(_ => _.Roles)
                .Include(_ => _.Members).ThenInclude(_ => _.Role)
                .Include(_ => _.Members).ThenInclude(_ => _.User)
                .FirstAsync(_ => _.Id == groupId);

            return Ok(new FullGroupData()
            {
                Id = group.Id,
                Name = group.Name,
                Manager = ModelMapper.Map<UserData>(group.Manager),
                Directors = ModelMapper.Map<IEnumerable<UserData>>(group.Directors),
                Roles = group.Roles.GroupJoin(group.Members, _ => _.Id, _ => _.RoleId,
                    (role, groupMember) => new GroupRoleData
                    {
                        Id = role.Id,
                        Section = role.Section,
                        Num = role.Num,
                        Members = ModelMapper.Map<IEnumerable<UserData>>(groupMember.Select(_ => _.User))
                    })
            });
        }
    }
}
