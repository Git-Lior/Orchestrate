﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : OrchestrateController
    {
        private readonly IPasswordProvider _passwordProvider;

        public UsersController(IPasswordProvider passwordProvider, IServiceProvider provider) : base(provider)
        {
            _passwordProvider = passwordProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] int groupId)
        {
            if (groupId <= 0)
                return Ok(ModelMapper.Map<IEnumerable<UserData>>(await DbContext.Users.AsNoTracking().ToListAsync()));

            var group = await DbContext.Groups.AsNoTracking()
                .Include(_ => _.Manager)
                .Include(_ => _.Directors)
                .Include(_ => _.AssignedRoles).ThenInclude(_ => _.User)
                .FirstOrDefaultAsync(_ => _.Id == groupId);

            var users = new List<User> { group.Manager }
                .Concat(group.Directors)
                .Concat(group.AssignedRoles.Select(_ => _.User))
                .Distinct();

            return Ok(ModelMapper.Map<IEnumerable<UserData>>(users));
        }

        [HttpPost]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> CreateUser([Bind("FirstName,LastName,Email")] User user)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            string password = _passwordProvider.GenerateTemporaryPassword(16);
            user.PasswordHash = _passwordProvider.HashPassword(password);
            user.IsPasswordTemporary = true;

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var userData = ModelMapper.Map<UserDataWithTemporaryPassword>(user);
            userData.TemporaryPassword = password;

            return Ok(userData);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [Bind("FirstName,LastName,Email")] User user)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            if (await DbContext.Users.AllAsync(_ => _.Id != user.Id)) return NotFound(new { Error = "User not found" });

            DbContext.Users.Update(user);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<UserData>(user));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = GroupRolesPolicy.AdministratorOnly)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (await DbContext.Groups.AsNoTracking().AnyAsync(_ => _.ManagerId == id))
                return BadRequest(new { Error = "Cannot delete a user that manages a group, change the group's manager first" });

            var user = await DbContext.Users.FindAsync(id);
            if (user == null) return NotFound(new { Error = "User not found" });

            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
