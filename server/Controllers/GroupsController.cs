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
            var user = await DbContext.Users.AsNoTracking()
                .Include(_ => _.MemberOfGroups).ThenInclude(_ => _.Group)
                .Include(_ => _.ManagingGroups)
                .Include(_ => _.DirectorOfGroups)
                .FirstOrDefaultAsync(_ => _.Id == RequestingUserId);

            if (user == null) throw new UserNotExistException();

            var groups = user.MemberOfGroups.Select(_ => _.Group)
                .Concat(user.ManagingGroups)
                .Concat(user.DirectorOfGroups)
                .OrderBy(_ => _.Name)
                .Distinct();

            return Ok(ModelMapper.Map<IEnumerable<GroupData>>(groups));
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
