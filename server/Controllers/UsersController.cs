using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministratorOnly")]
    public class UsersController : OrchestrateController
    {
        private readonly IPasswordProvider _passwordProvider;

        public UsersController(IPasswordProvider passwordProvider, IServiceProvider provider) : base(provider)
        {
            _passwordProvider = passwordProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var dbUsers = await DbContext.Users.AsNoTracking().ToListAsync();
            return Ok(ModelMapper.Map(dbUsers, new List<UserData>()));
        }

        [HttpPost]
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
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [Bind("FirstName,LastName,Email")] User user)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            if (await DbContext.Users.AllAsync(_ => _.Id != user.Id)) return NotFound(new { Error = "User not found" });

            DbContext.Users.Update(user);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<UserData>(user));
        }

        [HttpDelete("{id}")]
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
