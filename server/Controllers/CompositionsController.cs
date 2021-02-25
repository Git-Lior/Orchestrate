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
        public CompositionsController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetCompositions([FromRoute] int groupId, [FromQuery] string title, [FromQuery] string genre, [FromQuery] bool onlyInConcert)
        {
            var group = await DbContext.Groups
                .Include(_ => _.Concerts)
                .Include(_ => _.Compositions).ThenInclude(_ => _.Uploader)
                .Include(_ => _.Compositions).ThenInclude(_ => _.SheetMusics)
                .FirstAsync(_ => _.Id == groupId);

            IEnumerable<Composition> compositions = group.Compositions;

            // for members - show only compositions with relevant sheet music
            if (!IsUserDirector) compositions = compositions.Where(c => GetRelevantSheetMusic(c).Any());

            if (title != null) compositions = compositions.Where(_ => _.Title.Contains(title));
            if (genre != null) compositions = compositions.Where(_ => _.Genre == genre);

            if (onlyInConcert) { } /* TODO */

            return Ok(ModelMapper.Map<IEnumerable<CompositionData>>(compositions));
        }

        [HttpGet("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetFullComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var group = await DbContext.Groups
                .Include(_ => _.Concerts)
                .Include(_ => _.Compositions).ThenInclude(_ => _.Uploader)
                .Include(_ => _.Compositions).ThenInclude(_ => _.SheetMusics)
                .FirstAsync(_ => _.Id == groupId);

            var composition = group.Compositions.FirstOrDefault(_ => _.Id == compositionId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            return Ok(ModelMapper.Map<FullCompositionData>(composition));
        }

        [HttpGet("genres")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetGenres([FromRoute] int groupId)
        {
            var group = await DbContext.Groups.Include(_ => _.Compositions).AsNoTracking().FirstAsync(_ => _.Id == groupId);

            return Ok(group.Compositions.Select(_ => _.Genre).Distinct());
        }

        [HttpPost("")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
        public async Task<IActionResult> AddComposition([FromRoute] int groupId, [Bind("Title,Composer,Genre")] Composition composition)
        {
            if (!ModelState.IsValid) return BadRequest(new { Error = "Invalid parameters" });

            composition.UploaderId = RequestingUserId;

            var group = await DbContext.Groups.FindAsync(groupId);

            group.Compositions.Add(composition);
            await DbContext.SaveChangesAsync();

            return Ok(ModelMapper.Map<CompositionData>(composition));
        }

        [HttpDelete("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
        public async Task<IActionResult> RemoveComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var group = await DbContext.Groups.FindAsync(groupId);
            var composition = group.Compositions.FirstOrDefault(_ => _.Id == compositionId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            group.Compositions.Remove(composition);
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
