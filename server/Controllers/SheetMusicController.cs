using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/compositions/{compositionId}/roles/{roleId}")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
    public class SheetMusicController : OrchestrateController<SheetMusic>
    {
        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int CompositionId { get; set; }
        [FromRoute]
        public int RoleId { get; set; }

        protected override IQueryable<SheetMusic> MatchingEntityQuery(IQueryable<SheetMusic> query)
            => query.Where(_ => _.GroupId == GroupId
                             && _.CompositionId == CompositionId
                             && _.RoleId == RoleId);

        public SheetMusicController(IServiceProvider provider) : base(provider) { }

        [HttpGet("file")]
        public async Task<IActionResult> GetSheetMusicFile()
        {
            var sheetMusicFile = await MatchingEntityQuery(DbContext.SheetMusics).Select(_ => _.File).SingleOrDefaultAsync();

            if (sheetMusicFile == null) throw new ArgumentNullException("Sheet music does not exist");

            return File(sheetMusicFile, "application/pdf");
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments()
        {
            var sheetMusic = await MatchingEntityQuery(DbContext.SheetMusics)
                    .Include(_ => _.Comments).ThenInclude(_ => _.User)
                    .SingleOrDefaultAsync();

            if (sheetMusic == null) throw new ArgumentNullException("Sheet music does not exist");

            return Ok(ModelMapper.Map<IEnumerable<SheetMusicCommentData>>(sheetMusic.Comments));
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody, StringLength(300, MinimumLength = 1)] string content)
        {
            var sheetMusic = await GetMatchingEntity(DbContext.SheetMusics);

            if (sheetMusic == null) throw new ArgumentNullException("Sheet music does not exist");

            sheetMusic.Comments.Add(new SheetMusicComment
            {
                UserId = RequestingUserId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            });

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int commentId, [FromBody] string content)
        {
            var sheetMusic = await MatchingEntityQuery(DbContext.SheetMusics)
                .Include(_ => _.Comments.Where(_ => _.Id == commentId && _.UserId == RequestingUserId))
                .SingleOrDefaultAsync();

            var comment = sheetMusic?.Comments.SingleOrDefault();

            if (comment == null) throw new ArgumentNullException("Comment does not exist");

            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            var sheetMusic = await MatchingEntityQuery(DbContext.SheetMusics)
                .Include(_ => _.Comments.Where(_ => _.Id == commentId && _.UserId == RequestingUserId))
                .SingleOrDefaultAsync();

            var comment = sheetMusic?.Comments.SingleOrDefault();

            if (comment == null) throw new ArgumentNullException("Comment does not exist");

            comment.Content = null;
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
