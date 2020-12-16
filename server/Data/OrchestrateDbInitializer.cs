using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;

namespace Orchestrate.API.Data
{
    public class OrchestrateDbInitializer
    {
        private IWebHostEnvironment _env;
        private OrchestrateContext _ctx;

        public OrchestrateDbInitializer(IWebHostEnvironment env, OrchestrateContext ctx)
        {
            _env = env;
            _ctx = ctx;
        }

        public void Initialize()
        {
            _ctx.Database.Migrate();

            if (!_env.IsDevelopment()) return;

            // TODO: add seed data for development
            // _ctx.SaveChanges();
        }
    }
}
