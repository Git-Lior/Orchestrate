﻿using AutoMapper.QueryableExtensions;
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

            return Ok(await DbContext.Groups
                        .Where(_ => _.ManagerId == RequestingUserId
                                    || _.Directors.Any(_ => _.Id == RequestingUserId)
                                    || _.Roles.Any(_ => _.Members.Any(_ => _.Id == RequestingUserId)))
                        .OrderBy(_ => _.Name)
                        .ProjectTo<GroupData>(MapperConfig)
                        .ToListAsync());
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupInfo(int groupId)
        {
            return Ok(await DbContext.Groups.ProjectTo<FullGroupData>(MapperConfig).SingleAsync(_ => _.Id == groupId));
        }
    }
}
