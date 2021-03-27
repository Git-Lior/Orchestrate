using Microsoft.Extensions.DependencyInjection;
using Orchestrate.API.Services.Interfaces;
using Orchestrate.Data.Interfaces;
using System;

namespace Orchestrate.API.Services
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
