using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Data;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministratorOnly")]
    public class GroupsController : OrchestrateController
    {
        private readonly IMapper _mapper;
        private readonly OrchestrateContext _context;

        public GroupsController(IMapper mapper, OrchestrateContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Groups()
        {
            var groups = await _context.Groups
                .AsNoTracking()
                .Include(_ => _.Manager)
                .ToListAsync();

            return Ok(_mapper.Map(groups, new List<GroupData>()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([Bind("Name,Manager")] Group group)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<GroupData>(group));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup([FromRoute] int id, [Bind("Name,Manager")] Group group)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            if (await _context.Groups.AllAsync(_ => _.Id != group.Id)) return NotFound(new { Error = "Group not found" });

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound(new { Error = "Group not found" });

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
