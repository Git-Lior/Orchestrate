﻿using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
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
    public class GroupManagerController : OrchestrateController
    {
        public GroupManagerController(IServiceProvider provider) : base(provider) { }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await DbContext.Users.AsNoTracking().ProjectTo<UserData>(MapperConfig).ToListAsync());
        }

        [HttpGet("updates")]
        public async Task<IActionResult> GetUpdates([FromRoute] int groupId)
        {
            var timeBound = DateTime.Today.AddDays(-7);

            var compositions = await DbContext.Compositions.AsNoTracking()
                .Where(c => c.GroupId == groupId && c.CreatedAt > timeBound)
                .ProjectTo<CompositionUpdateData>(MapperConfig)
                .ToListAsync();

            var concerts = await DbContext.Concerts.AsNoTracking()
                .Include(c => c.Attendances.Where(_ => _.UpdatedAt > timeBound))
                .Where(c => c.Attendances.Count > 0)
                .ToListAsync();

            IEnumerable<dynamic> concertDatas = concerts
                .Select(c => new ConcertUpdateData
                {
                    Date = ModelMapper.Map<long>(c.Attendances.Max(_ => _.UpdatedAt)),
                    Concert = ModelMapper.Map<BasicConcertData>(c),
                    Attendance = c.Attendances.Count
                });

            return Ok(concertDatas.Concat(compositions).OrderByDescending(_ => _.Date));
        }

        [HttpPost("directors")]
        public async Task<IActionResult> AddDirector([FromRoute] int groupId, [FromBody] int directorId)
        {
            var director = await DbContext.Users.FindAsync(directorId);
            if (director == null) throw new ArgumentException("User not found");

            var group = await DbContext.Groups
                .Include(_ => _.Directors.Where(_ => _.Id == directorId))
                .SingleAsync(_ => _.Id == groupId);
            if (group.Directors.Any()) throw new ArgumentException("Director already in group");

            group.Directors.Add(director);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("directors/{directorId}")]
        public async Task<IActionResult> RemoveDirector([FromRoute] int groupId, [FromRoute] int directorId)
        {
            var director = await DbContext.Users.FindAsync(directorId);
            if (director == null) throw new ArgumentException("User not found");

            var group = await DbContext.Groups
                .Include(_ => _.Directors.Where(_ => _.Id == directorId))
                .SingleAsync(_ => _.Id == groupId);
            if (!group.Directors.Any()) throw new ArgumentException("Director does not exist in group");

            group.Directors.Remove(director);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromRoute] int groupId, [FromBody] RolePayload role)
        {
            var group = await DbContext.Groups.Include(_ => _.Roles).SingleAsync(_ => _.Id == groupId);
            var dbRole = await DbContext.Roles.SingleOrDefaultAsync(_ => _.Section == role.Section && _.Num == role.Num);

            if (dbRole == null)
            {
                dbRole = ModelMapper.Map<Role>(role);
                DbContext.Roles.Add(dbRole);
                await DbContext.SaveChangesAsync();
            }
            else if (group.Roles.Any(_ => _.RoleId == dbRole.Id))
                throw new ArgumentException("Role already exists in group");

            var groupRole = new GroupRole { Role = dbRole };
            group.Roles.Add(groupRole);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<GroupRoleData>(groupRole));
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> RemoveRole([FromRoute] int groupId, [FromRoute] int roleId)
        {
            var groupRole = await DbContext.GroupRoles
                .Include(_ => _.Role)
                .SingleAsync(_ => _.GroupId == groupId && _.RoleId == roleId);

            var groupCount = await DbContext.Groups.Where(_ => _.Roles.Any(_ => _.RoleId == roleId)).CountAsync();

            if (groupCount <= 1) DbContext.Remove(groupRole.Role);
            else DbContext.Remove(groupRole);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("roles/{roleId}/members")]
        public async Task<IActionResult> AddMember([FromRoute] int groupId, [FromRoute] int roleId, [FromBody] int memberId)
        {
            var dbUser = await DbContext.Users.FindAsync(memberId);
            if (dbUser == null) throw new ArgumentException("User doesn't exist");

            var groupRole = await DbContext.GroupRoles
                .Include(_ => _.Members)
                .SingleAsync(_ => _.GroupId == groupId && _.RoleId == roleId);

            groupRole.Members.Add(dbUser);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("roles/{roleId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] int groupId, [FromRoute] int roleId, [FromRoute] int memberId)
        {
            var groupRole = await DbContext.GroupRoles
                .Include(_ => _.Members.Where(_ => _.Id == memberId))
                .SingleAsync(_ => _.GroupId == groupId && _.RoleId == roleId);

            if (!groupRole.Members.Any()) throw new ArgumentException("Member doesn't exist in this role");

            groupRole.Members.Clear();
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
