using Orchestrate.Data.Interfaces;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IEntityRepositoryProvider
    {
        IEntityRepository<T> Get<T>() where T : class;
    }
}
