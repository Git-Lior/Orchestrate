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
    [Route("api/groups/{groupId}/compositions/{compositionId}/roles/{roleId}")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
    public class SheetMusicController : OrchestrateController
    {
        [BindProperty]
        private int GroupId { get; set; }
        [BindProperty]
        private int CompositionId { get; set; }
        [BindProperty]
        private int RoleId { get; set; }

        private IQueryable<SheetMusic> SheetMusicQuery =>
            DbContext.SheetMusics.Where(_ => _.GroupId == GroupId
                                            && _.CompositionId == CompositionId
                                            && _.RoleId == RoleId);

        public SheetMusicController(IServiceProvider provider) : base(provider) { }

        [HttpGet("/file")]
        public async Task<IActionResult> GetSheetMusicFile()
        {
            var sheetMusicFile = await SheetMusicQuery.Select(_ => _.File).SingleOrDefaultAsync();

            if (sheetMusicFile == null) throw new ArgumentNullException("Sheet music does not exist");

            return File(sheetMusicFile, "application/pdf");
        }

        [HttpGet("/comments")]
        public async Task<IActionResult> GetComments()
        {
            var sheetMusic = await SheetMusicQuery.Include(_ => _.Comments).ThenInclude(_ => _.User)
                .SingleOrDefaultAsync();

            if (sheetMusic == null) throw new ArgumentNullException("Sheet music does not exist");

            return Ok(ModelMapper.Map<IEnumerable<SheetMusicCommentData>>(sheetMusic.Comments));
        }

        [HttpPost("/comments")]
        public async Task<IActionResult> AddComment([FromBody] string content)
        {
            var sheetMusic = await SheetMusicQuery.SingleOrDefaultAsync();

            if (sheetMusic == null) throw new ArgumentNullException("Sheet music does not exist");

            sheetMusic.Comments.Add(new SheetMusicComment
            {
                UserId = RequestingUserId,
                Content = content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int commentId, [FromBody] string content)
        {
            var comment = await SheetMusicQuery
                .SelectMany(_ => _.Comments.Where(_ => _.UserId == RequestingUserId))
                .Where(_ => _.Id == commentId)
                .SingleOrDefaultAsync();

            if (comment == null) throw new ArgumentNullException("Comment does not exist");

            comment.Content = content;
            comment.UpdatedAt = DateTime.Now;

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/comment/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            var comment = await SheetMusicQuery
                .SelectMany(_ => _.Comments.Where(_ => _.UserId == RequestingUserId))
                .Where(_ => _.Id == commentId)
                .SingleOrDefaultAsync();

            if (comment == null) throw new ArgumentNullException("Comment does not exist");

            DbContext.Remove(comment);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
