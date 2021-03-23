using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrate.API.Data;
using Orchestrate.API.Services.Interfaces;
using System;

namespace Orchestrate.API.Controllers.Helpers
{
    [Authorize]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IMapper ModelMapper { get; }
        protected OrchestrateContext DbContext { get; }
        protected IUserGroupPositionProvider UserGroupPosition { get; }

        protected int RequestingUserId => int.Parse(User.Identity.Name);
        protected IConfigurationProvider MapperConfig => ModelMapper.ConfigurationProvider;

        public ApiControllerBase(IServiceProvider provider)
        {
            ModelMapper = provider.GetRequiredService<IMapper>();
            DbContext = provider.GetRequiredService<OrchestrateContext>();
            UserGroupPosition = provider.GetRequiredService<IUserGroupPositionProvider>();
        }
    }
}
