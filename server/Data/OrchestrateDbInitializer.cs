using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Data
{
    public class OrchestrateDbInitializer
    {
        private const string TEST_USER_EMAIL = "test@test.com";
        private IWebHostEnvironment _env;
        private OrchestrateContext _ctx;

        public OrchestrateDbInitializer(IWebHostEnvironment env, OrchestrateContext ctx)
        {
            _env = env;
            _ctx = ctx;
        }

        public async Task Initialize()
        {
            _ctx.Database.Migrate();

            var testUser = await _ctx.Users.FirstOrDefaultAsync(_ => _.Email == TEST_USER_EMAIL);
            if (testUser == null)
                _ctx.Users.Add(new User
                {
                    Email = TEST_USER_EMAIL,
                    PasswordHash = "AQAAAAEAACcQAAAAEE8iFfjLAcKd4RJUnIPSV4h0XfbKOJELDHMazdN6fcwZ+Z4TieAMz/sNfX0+9X8TNQ==", // password: "test"
                    FirstName = "משתמש",
                    LastName = "בדיקה",
                });

            _ctx.SaveChanges();
        }
    }
}
