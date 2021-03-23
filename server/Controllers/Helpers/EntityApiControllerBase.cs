using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Helpers
{
    public abstract class EntityApiControllerBase<T> : ApiControllerBase
    {
        public EntityApiControllerBase(IServiceProvider provider) : base(provider) { }

        protected virtual string EntityName => "Entity";
        protected virtual IQueryable<T> MatchingEntityQuery(IQueryable<T> query) => query;

        protected async Task<T> GetMatchingEntity(IQueryable<T> source)
        {
            var result = await MatchingEntityQuery(source).SingleOrDefaultAsync();

            if (result == null) throw new ArgumentException($"{EntityName} does not exist");

            return result;
        }

        protected async Task<S> GetMatchingEntity<S>(IQueryable<T> source, Func<IQueryable<T>, IQueryable<S>> targetQuery)
        {
            var result = await targetQuery(MatchingEntityQuery(source)).SingleOrDefaultAsync();

            if (result == null) throw new ArgumentException($"{EntityName} does not exist");

            return result;
        }
    }
}
