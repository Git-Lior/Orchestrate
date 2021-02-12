using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Data;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministratorOnly")]
    public class UsersController : OrchestrateController
    {
        private readonly IMapper _mapper;
        private readonly OrchestrateContext _context;
        private readonly IPasswordProvider _passwordProvider;

        public UsersController(IMapper mapper, OrchestrateContext context, IPasswordProvider passwordProvider)
        {
            _mapper = mapper;
            _context = context;
            _passwordProvider = passwordProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var dbUsers = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map(dbUsers, new List<UserData>()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([Bind("FirstName,LastName,Email")] User user)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            string password = _passwordProvider.GenerateTemporaryPassword(16);
            user.PasswordHash = _passwordProvider.HashPassword(password);
            user.IsPasswordTemporary = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userData = _mapper.Map<UserDataWithTemporaryPassword>(user);
            userData.TemporaryPassword = password;

            return Ok(userData);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [Bind("FirstName,LastName,Email")] User user)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            if (await _context.Users.AllAsync(_ => _.Id != user.Id)) return NotFound(new { Error = "User not found" });

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserData>(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (await _context.Groups.AsNoTracking().AnyAsync(_ => _.ManagerId == id))
                return BadRequest(new { Error = "Cannot delete a user that manages a group, change the group's manager first" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { Error = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
