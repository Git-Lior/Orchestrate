using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Models;
using Orchestrate.Data.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories
{
    public class SheetMusicRepository : EntityRepositoryBase<SheetMusic>, ISheetMusicsRepository
    {
        protected override DbSet<SheetMusic> DbEntities => Context.SheetMusics;

        public SheetMusicRepository(OrchestrateContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<SheetMusic> FindOne(object identifier)
        {
            if (identifier is not SheetMusicIdentifier s) throw new InvalidOperationException("invalid sheet music identifier");

            return DbEntities.Where(_ => _.GroupId == s.GroupId && _.CompositionId == s.CompositionId && _.RoleId == s.RoleId);
        }

        public async Task AddComment(SheetMusic sheetMusic, int userId, string content)
        {
            sheetMusic.Comments.Add(new SheetMusicComment { UserId = userId, Content = content, CreatedAt = DateTime.UtcNow });

            await Context.SaveChangesAsync();
        }

        public async Task UpdateComment(SheetMusic sheetMusic, int userId, int commentId, string content)
        {
            var comment = sheetMusic.Comments.SingleOrDefault(_ => _.Id == commentId && _.UserId == userId);

            if (comment?.Content == null) throw new ArgumentException("Comment does not exist");

            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteComment(SheetMusic sheetMusic, int userId, int commentId)
        {
            var comment = sheetMusic.Comments.SingleOrDefault(_ => _.Id == commentId && _.UserId == userId);

            if (comment?.Content == null) throw new ArgumentException("Comment does not exist");

            comment.Content = null;

            await Context.SaveChangesAsync();
        }
    }
}
