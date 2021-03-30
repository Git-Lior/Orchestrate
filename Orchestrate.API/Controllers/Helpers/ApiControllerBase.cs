using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Helpers
{
    [Authorize]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IMapper Mapper { get; }
        protected IConfigurationProvider MapperConfig => Mapper.ConfigurationProvider;
        protected IEntityRepositoryProvider Repository { get; }

        protected int RequestingUserId => int.Parse(User.Identity.Name);
        protected IUserGroupPositionProvider UserGroupPosition { get; }

        public ApiControllerBase(IServiceProvider provider)
        {
            Mapper = provider.GetRequiredService<IMapper>();
            UserGroupPosition = provider.GetRequiredService<IUserGroupPositionProvider>();
            Repository = provider.GetRequiredService<IEntityRepositoryProvider>();
        }

        protected async Task<T> SingleOrError<T>(IQueryable<T> query, string typeName = null)
        {
            var entity = await query.SingleOrDefaultAsync();
            if (entity == null) throw new ArgumentException($"{typeName ?? typeof(T).Name} does not exist");

            return entity;
        }
    }
}
