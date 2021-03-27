using Orchestrate.Data.Interfaces;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IEntityRepositoryCreator
    {
        public IEntityRepository<T> Get<T>() where T : class;
    }
}
