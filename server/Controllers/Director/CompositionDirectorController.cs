using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestrate.API.Authorization;
using Orchestrate.API.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Director
{
    [Route("api/groups/{groupId}/compositions")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
    public class CompositionDirectorController : OrchestrateController<Composition>
    {
        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int CompositionId { get; set; }

        protected override IQueryable<Composition> MatchingEntityQuery(IQueryable<Composition> query)
            => query.Where(_ => _.GroupId == GroupId && _.Id == CompositionId);

        public CompositionDirectorController(IServiceProvider provider) : base(provider) { }

        [HttpPost]
        public async Task<IActionResult> AddComposition([FromBody] CompositionPayload payload)
        {
            var composition = ModelMapper.Map<Composition>(payload);
            composition.GroupId = GroupId;
            composition.UploaderId = RequestingUserId;
            composition.CreatedAt = DateTime.UtcNow;

            DbContext.Compositions.Add(composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{compositionId}")]
        public async Task<IActionResult> UpdateComposition([FromBody] CompositionPayload payload)
        {
            var composition = await GetMatchingEntity(DbContext.Compositions);
            if (composition == null) throw new ArgumentException("Composition doesn't exist");

            ModelMapper.Map(payload, composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{compositionId}")]
        public async Task<IActionResult> RemoveComposition()
        {
            var composition = await GetMatchingEntity(DbContext.Compositions);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            DbContext.Compositions.Remove(composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{compositionId}/{roleId}")]
        public async Task<IActionResult> UploadSheetMusicFile([FromRoute] int roleId, IFormFile file)
        {
            using var stream = new MemoryStream((int)file.Length);
            await file.CopyToAsync(stream);

            var sheetMusic = await DbContext.SheetMusics.FindAsync(GroupId, CompositionId, roleId);

            if (sheetMusic == null)
            {
                sheetMusic = new SheetMusic
                {
                    GroupId = GroupId,
                    CompositionId = CompositionId,
                    RoleId = roleId
                };
                DbContext.SheetMusics.Add(sheetMusic);
            }

            sheetMusic.File = stream.ToArray();

            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
