using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CompositionDirectorController : OrchestrateController
    {
        public CompositionDirectorController(IServiceProvider provider) : base(provider) { }

        [HttpPost]
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
        public async Task<IActionResult> UpdateComposition([FromRoute] int groupId, [FromRoute] int compositionId, [FromBody] CompositionPayload payload)
        {
            var composition = await DbContext.Compositions.FindAsync(groupId, compositionId);
            if (composition == null) throw new ArgumentException("Composition doesn't exist");

            ModelMapper.Map(payload, composition);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{compositionId}")]
        public async Task<IActionResult> RemoveComposition([FromRoute] int groupId, [FromRoute] int compositionId)
        {
            var composition = await DbContext.Compositions.FindAsync(groupId, compositionId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            DbContext.Compositions.Remove(composition);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{compositionId}/roles")]
        public async Task<IActionResult> SetCompositionRoles([FromRoute] int groupId, [FromRoute] int compositionId, [FromBody] int[] roleIds)
        {
            var composition = await DbContext.Compositions
                .Include(_ => _.SheetMusics)
                .FirstOrDefaultAsync(_ => _.GroupId == groupId && _.Id == compositionId);

            if (composition == null) throw new ArgumentException("Composition does not exist");

            composition.SheetMusics = roleIds.GroupJoin(composition.SheetMusics, _ => _, _ => _.RoleId,
                (roleId, sheetmusics) => sheetmusics.SingleOrDefault() ?? new SheetMusic
                {
                    RoleId = roleId
                }).ToList();

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{compositionId}/{roleId}/file")]
        public async Task<IActionResult> UploadSheetMusicFile([FromRoute] int groupId, [FromRoute] int compositionId, [FromRoute] int roleId, IFormFile file)
        {
            using var stream = new MemoryStream((int)file.Length);
            await file.CopyToAsync(stream);

            var sheetMusic = await DbContext.SheetMusics.FindAsync(groupId, compositionId, roleId);

            if (sheetMusic == null)
            {
                sheetMusic = new SheetMusic
                {
                    GroupId = groupId,
                    CompositionId = compositionId,
                    RoleId = roleId,
                    File = stream.ToArray()
                };
                DbContext.SheetMusics.Add(sheetMusic);
            }

            sheetMusic.File = stream.ToArray();

            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
