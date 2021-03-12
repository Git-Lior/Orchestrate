using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/compositions")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
    public class CompositionsController : OrchestrateController
    {
        public CompositionsController(IServiceProvider provider) : base(provider) { }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres([FromRoute] int groupId)
        {
            return Ok(await DbContext.Compositions
                .Where(_ => _.GroupId == groupId)
                .Select(_ => _.Genre).Distinct()
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCompositions([FromRoute] int groupId, [FromQuery] string title, [FromQuery] string genre, [FromQuery] bool onlyInUpcomingConcert)
        {
            var compositions = DbContext.Compositions
                .Where(c => c.GroupId == groupId
                            && (title == null || c.Title.Contains(title))
                            && (genre == null || c.Genre == genre));

            if (onlyInUpcomingConcert)
            {
                compositions = compositions.Intersect(
                    DbContext.Concerts
                        .Where(_ => _.GroupId == groupId && _.Date > DateTime.Now)
                        .SelectMany(_ => _.Compositions)
                );
            }

            if (IsUserDirector)
                return Ok(await compositions.ProjectTo<CompositionData>(MapperConfig).ToListAsync());

            IEnumerable<Composition> result = await compositions
                .Include(_ => _.Uploader)
                .Include(_ => _.SheetMusics)
                .AsNoTracking().ToListAsync();

            var rolesSet = new HashSet<int>(MemberRoles.Select(_ => _.Id));

            result = result.Where(c => c.SheetMusics.Any(s => rolesSet.Contains(s.RoleId)));

            return Ok(ModelMapper.Map<IEnumerable<CompositionData>>(result));
        }

        [HttpGet("{compositionId}")]
        public async Task<IActionResult> GetFullComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions
                .Where(c => c.GroupId == groupId && c.Id == compositionId)
                .Include(c => c.SheetMusics.Where(s => MemberRoles.Any(r => r.Id == s.RoleId)))
                .ProjectTo<FullCompositionData>(MapperConfig)
                .FirstOrDefaultAsync();

            if (composition == null) throw new ArgumentException("Composition does not exist");

            return Ok(composition);
        }
    }
}
