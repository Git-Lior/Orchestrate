using Microsoft.Extensions.DependencyInjection;
using Orchestrate.API.Data.Repositories.Interfaces;
using System;

namespace Orchestrate.API.Data.Repositories
{
    public class EntityRepositoryCreator : IEntityRepositoryCreator
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityRepositoryCreator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEntityRepository<T> Get<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<IEntityRepository<T>>();
        }
    }
}
