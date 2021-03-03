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
                            && (genre == null || c.Genre == genre)
                            && (IsUserDirector || IsCompositionRelevantToMember(c)));

            if (onlyInUpcomingConcert)
            {
                compositions = compositions.Intersect(
                    DbContext.Concerts.Where(_ => _.GroupId == groupId && _.Date > DateTime.Now)
                        .SelectMany(_ => _.ConcertCompositions)
                        .Select(_ => _.Composition)
                );
            }

            return Ok(await compositions.ProjectTo<CompositionData>(MapperConfig).ToListAsync());
        }

        [HttpGet("{compositionId}")]
        public async Task<IActionResult> GetFullComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions
                .Where(c => c.GroupId == groupId && c.Id == compositionId
                            && (IsUserDirector || IsCompositionRelevantToMember(c)))
                .ProjectTo<FullCompositionData>(MapperConfig)
                .FirstOrDefaultAsync();

            if (composition == null) throw new ArgumentException("Composition does not exist");

            return Ok(composition);
        }

        private bool IsCompositionRelevantToMember(Composition c) => c.SheetMusics.Any(s => MemberRoles.Any(r => r.Id == s.RoleId));
    }
}
