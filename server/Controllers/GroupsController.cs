using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Models;
using Orchestrate.Data.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class GroupsController : ApiControllerBase
    {
        private readonly IEntityRepository<Group> _groupsRepo;
        public GroupsController(IServiceProvider provider) : base(provider)
        {
            _groupsRepo = Repository.Get<Group>();
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            return Ok(await _groupsRepo.NoTrackedEntities
                        .Where(_ => _.ManagerId == RequestingUserId
                                    || _.Directors.Any(_ => _.Id == RequestingUserId)
                                    || _.Roles.Any(_ => _.Members.Any(_ => _.Id == RequestingUserId)))
                        .OrderBy(_ => _.Name)
                        .ProjectTo<GroupData>(MapperConfig)
                        .ToListAsync());
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupInfo([FromRoute] int groupId)
        {
            return Ok(await SingleOrError(_groupsRepo.FindOne(new GroupIdentifier(groupId)).ProjectTo<FullGroupData>(MapperConfig)));
        }
    }
}
