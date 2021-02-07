using Microsoft.Extensions.Configuration;
using Orchestrate.API.Services.Interfaces;
using System;

namespace Orchestrate.API.Services
{
    public class AdminService : IAdminService
    {
        private IConfiguration _config;

        public AdminService(IConfiguration config)
        {
            _config = config;
        }

        public string Authenticate(string password)
        {
            if (password != _config.GetValue<string>("AdminPassword"))
                throw new ArgumentException("Invalid admin credentials");
            return _config.GetValue<string>("AdminToken");
        }

        public void Verify(string token)
        {
            if (token != _config.GetValue<string>("AdminToken"))
                throw new ArgumentException("Invalid admin credentials");
        }
    }
}
