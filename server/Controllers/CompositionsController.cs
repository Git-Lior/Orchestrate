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
    public class CompositionsController : OrchestrateController
    {
        public CompositionsController(IServiceProvider provider) : base(provider) { }

        [HttpGet("genres")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetGenres([FromRoute] int groupId)
        {
            var group = await DbContext.Groups.Include(_ => _.Compositions).AsNoTracking().FirstAsync(_ => _.Id == groupId);

            return Ok(group.Compositions.Select(_ => _.Genre).Distinct());
        }

        [HttpGet]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetCompositions([FromRoute] int groupId, [FromQuery] string title, [FromQuery] string genre, [FromQuery] bool onlyInUpcomingConcert)
        {
            var group = await DbContext.Groups
                .Include(_ => _.Compositions).ThenInclude(_ => _.Uploader)
                .Include(_ => _.Concerts.Where(c => c.Date > DateTime.Now))
                    .ThenInclude(_ => _.ConcertCompositions)
                .FirstAsync(_ => _.Id == groupId);

            IEnumerable<Composition> compositions = group.Compositions;

            // for members - show only compositions with relevant sheet music
            if (!IsUserDirector) compositions = compositions.Where(c => GetRelevantSheetMusic(c).Any());

            if (title != null) compositions = compositions.Where(_ => _.Title.Contains(title));
            if (genre != null) compositions = compositions.Where(_ => _.Genre == genre);

            if (onlyInUpcomingConcert)
            {
                var upcomingConcertIds = group.Concerts.SelectMany(_ => _.ConcertCompositions.Select(_ => _.CompositionId)).ToHashSet();
                compositions = compositions.Where(_ => upcomingConcertIds.Contains(_.Id));
            }

            return Ok(ModelMapper.Map<IEnumerable<CompositionData>>(compositions));
        }

        [HttpGet("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetFullComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions
                .Include(_ => _.Uploader)
                .Include(_ => _.SheetMusics)
                .FirstOrDefaultAsync(_ => _.Id == compositionId && _.GroupId == groupId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            return Ok(ModelMapper.Map<FullCompositionData>(composition));
        }

        [HttpPost]
        [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
        public async Task<IActionResult> AddComposition([FromRoute] int groupId, [FromBody] CompositionPayload payload)
        {
            var composition = ModelMapper.Map<Composition>(payload);
            composition.GroupId = groupId;
            composition.UploaderId = RequestingUserId;

            DbContext.Compositions.Add(composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
        public async Task<IActionResult> UpdateComposition([FromRoute] int groupId, [FromRoute] int compositionId, [FromBody] CompositionPayload payload)
        {
            var composition = await DbContext.Compositions.FindAsync(groupId, compositionId);
            if (composition == null) throw new ArgumentException("Composition doesn't exist");

            ModelMapper.Map(payload, composition);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
        public async Task<IActionResult> RemoveComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions.FindAsync(groupId, compositionId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            DbContext.Compositions.Remove(composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        private IEnumerable<SheetMusic> GetRelevantSheetMusic(Composition c)
        {
            var rolesSet = new HashSet<int>(MemberRoles.Select(_ => _.Id));
            return c.SheetMusics.Where(_ => rolesSet.Contains(_.RoleId));
        }
    }
}
