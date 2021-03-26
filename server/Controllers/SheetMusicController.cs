using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.Data.Repositories;
using Orchestrate.API.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/compositions/{compositionId}/roles/{roleId}")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
    public class SheetMusicController : ApiControllerBase
    {
        private readonly SheetMusicRepository _sheetMusicRepo;

        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int CompositionId { get; set; }
        [FromRoute]
        public int RoleId { get; set; }

        private SheetMusicIdentifier EntityId => new SheetMusicIdentifier(GroupId, CompositionId, RoleId);

        public SheetMusicController(IServiceProvider provider, SheetMusicRepository sheetMusicRepo) : base(provider)
        {
            _sheetMusicRepo = sheetMusicRepo;
        }

        [HttpGet("file")]
        public async Task<IActionResult> GetSheetMusicFile()
        {
            var sheetMusic = await SingleOrError(_sheetMusicRepo.FindOne(EntityId), "Sheet Music");

            return File(sheetMusic.File, "application/pdf");
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments()
        {
            var sheetMusic = await SingleOrError(_sheetMusicRepo
                .FindOne(EntityId)
                .Include(_ => _.Comments)
                    .ThenInclude(_ => _.User), "Sheet Music");

            return Ok(Mapper.Map<IEnumerable<SheetMusicCommentData>>(sheetMusic.Comments));
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody, StringLength(300, MinimumLength = 1)] string content)
        {
            var sheetMusic = await SingleOrError(_sheetMusicRepo.FindOne(EntityId), "Sheet Music");

            await _sheetMusicRepo.AddComment(sheetMusic, RequestingUserId, content);

            return Ok();
        }

        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int commentId, [FromBody] string content)
        {
            var sheetMusic = await SingleOrError(_sheetMusicRepo.FindOne(EntityId), "Sheet Music");

            await _sheetMusicRepo.UpdateComment(sheetMusic, RequestingUserId, commentId, content);

            return Ok();
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            var sheetMusic = await SingleOrError(_sheetMusicRepo.FindOne(EntityId), "Sheet Music");

            await _sheetMusicRepo.DeleteComment(sheetMusic, RequestingUserId, commentId);

            return Ok();
        }
    }
}
