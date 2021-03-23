using AutoMapper.QueryableExtensions;
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

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/compositions")]
    public class CompositionsController : ApiControllerBase
    {
        public CompositionsController(IServiceProvider provider) : base(provider) { }

        [HttpGet("genres")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
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
                        .Where(_ => _.GroupId == groupId && _.Date > DateTime.UtcNow)
                        .SelectMany(_ => _.Compositions)
                );
            }

            if (UserGroupPosition.Director || UserGroupPosition.Manager)
                return Ok(await compositions.ProjectTo<CompositionData>(MapperConfig).ToListAsync());

            IEnumerable<Composition> result = await compositions.AsNoTracking()
                .Include(_ => _.Uploader)
                .Include(_ => _.SheetMusics)
                .ToListAsync();

            var rolesSet = new HashSet<int>(UserGroupPosition.Roles.Select(_ => _.Id));

            result = result.Where(c => c.SheetMusics.Any(s => rolesSet.Contains(s.RoleId)));

            return Ok(ModelMapper.Map<IEnumerable<CompositionData>>(result));
        }

        [HttpGet("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetFullComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions
                .Where(c => c.GroupId == groupId && c.Id == compositionId)
                .Include(c => c.Uploader)
                .Include(c => c.SheetMusics
                    .Where(s => UserGroupPosition.Director
                             || UserGroupPosition.Roles.Any(r => r.Id == s.RoleId)))
                .SingleOrDefaultAsync();

            if (composition == null) throw new ArgumentException("Composition does not exist");

            return Ok(ModelMapper.Map<FullCompositionData>(composition));
        }
    }
}
