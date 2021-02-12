﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrate.API.Authorization;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Authorize]
    [ApiController]
    public class OrchestrateController : ControllerBase
    {
        private readonly string _adminRoleName;

        protected IMapper ModelMapper { get; }
        protected OrchestrateContext DbContext { get; }
        
        protected bool IsUserAdmin => User.IsInRole(_adminRoleName);
        protected int RequestingUserId => int.Parse(User.Identity.Name);

        public OrchestrateController(IServiceProvider provider)
        {
            ModelMapper = provider.GetRequiredService<IMapper>();
            DbContext = provider.GetRequiredService<OrchestrateContext>();
            _adminRoleName = provider.GetRequiredService<IOptions<JwtOptions>>().Value.AdminRoleName;
        }

        protected async Task<User> GetRequestingUser()
        {
            var user = await DbContext.Users.FindAsync(RequestingUserId);
            if (user == null) throw new UserNotExistException();

            return user;
        }
    }
}
