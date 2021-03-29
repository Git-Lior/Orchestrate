using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Interfaces
{
    public interface IEntityRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        IQueryable<T> NoTrackedEntities { get; }

        IQueryable<T> FindOne(object identifier);
        Task<T> Create(object payload);
        Task<T> Update(T entity, object payload);
        Task Delete(T entity);
    }
}
