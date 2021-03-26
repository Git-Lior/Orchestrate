namespace Orchestrate.API.Data.Repositories.Interfaces
{
    public interface IEntityRepositoryCreator
    {
        public IEntityRepository<T> Get<T>() where T : class;
    }
}
