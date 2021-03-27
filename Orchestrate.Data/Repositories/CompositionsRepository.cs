using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Repositories.Interfaces;
using Orchestrate.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories
{
    public class CompositionsRepository : EntityRepositoryBase<Composition>, ICompositionsRepository
    {
        protected override DbSet<Composition> DbEntities => Context.Compositions;

        public CompositionsRepository(OrchestrateContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<Composition> FindOne(object identifier)
        {
            if (identifier is not CompositionIdentifier c) throw new InvalidOperationException("invalid composition id");

            return DbEntities.Where(_ => _.GroupId == c.GroupId && _.Id == c.CompositionId);
        }

        public async Task UploadSheetMusic(Composition composition, int roleId, byte[] file)
        {
            var sheetMusic = await Context.SheetMusics.FindAsync(composition.GroupId, composition.Id, roleId);

            if (sheetMusic == null)
            {
                sheetMusic = new SheetMusic { GroupId = composition.GroupId, CompositionId = composition.Id, RoleId = roleId };
                Context.SheetMusics.Add(sheetMusic);
            }

            sheetMusic.File = file;
            await Context.SaveChangesAsync();
        }
    }
}
