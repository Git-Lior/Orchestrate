using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Interfaces
{
    public interface IEntityRepository<T> where T : class
    {
        public IQueryable<T> Entities { get; }
        public IQueryable<T> NoTrackedEntities { get; }

        public IQueryable<T> FindOne(object identifier);
        public IQueryable<T> FindOneNoTracked(object identifier);

        public Task<T> Create(object payload);
        public Task<TResult> Create<TResult>(object payload);

        public Task<T> Update(T entity, object payload);
        public Task<TResult> Update<TResult>(T entity, object payload);

        public Task Delete(T entity);
    }
}
