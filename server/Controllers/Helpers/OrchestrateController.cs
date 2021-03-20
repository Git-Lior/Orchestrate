using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Helpers
{
    [Authorize]
    [ApiController]
    public class OrchestrateController : ControllerBase
    {
        private readonly string _adminRoleName;

        protected IMapper ModelMapper { get; }
        protected OrchestrateContext DbContext { get; }

        protected IConfigurationProvider MapperConfig => ModelMapper.ConfigurationProvider;

        protected int RequestingUserId => int.Parse(User.Identity.Name);
        protected bool IsUserAdmin => User.IsInRole(_adminRoleName);
        protected bool IsUserManager => (bool)HttpContext.Items["IsUserManager"];
        protected bool IsUserDirector => (bool)HttpContext.Items["IsUserDirector"];
        protected IEnumerable<Role> MemberRoles => (IEnumerable<Role>)HttpContext.Items["MemberRoles"];

        public OrchestrateController(IServiceProvider provider)
        {
            ModelMapper = provider.GetRequiredService<IMapper>();
            DbContext = provider.GetRequiredService<OrchestrateContext>();
            _adminRoleName = provider.GetRequiredService<IOptions<JwtOptions>>().Value.AdminRoleName;
        }
    }

    public class OrchestrateController<T> : OrchestrateController
    {
        public OrchestrateController(IServiceProvider provider) : base(provider) { }

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
