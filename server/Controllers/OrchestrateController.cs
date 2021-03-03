using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;

namespace Orchestrate.API.Controllers
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
}
