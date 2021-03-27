using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories
{
    public abstract class EntityRepositoryBase<T> : IEntityRepository<T> where T : class
    {
        protected OrchestrateContext Context { get; }
        protected IMapper Mapper { get; }
        protected abstract DbSet<T> DbEntities { get; }

        public IQueryable<T> Entities => DbEntities;
        public IQueryable<T> NoTrackedEntities => DbEntities.AsNoTracking();

        protected EntityRepositoryBase(OrchestrateContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public abstract IQueryable<T> FindOne(object identifier);

        public IQueryable<T> FindOneNoTracked(object identifier) => FindOne(identifier).AsNoTracking();

        public virtual async Task<T> Create(object payload)
        {
            var entity = Mapper.Map<T>(payload);
            DbEntities.Add(entity);

            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<TResult> Create<TResult>(object payload)
        {
            var result = await Create(payload);
            return Mapper.Map<TResult>(result);
        }

        public virtual Task Delete(T entity)
        {
            DbEntities.Remove(entity);
            return Context.SaveChangesAsync();
        }

        public virtual async Task<T> Update(T entity, object payload)
        {
            Mapper.Map(payload, entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<TResult> Update<TResult>(T entity, object payload)
        {
            var result = await Update(entity, payload);
            return Mapper.Map<TResult>(result);
        }
    }
}
