using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Services.Interfaces;
using Orchestrate.Data;
using Orchestrate.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Services
{
    public class UserGroupPositionProvider : IUserGroupPositionProvider
    {
        private readonly OrchestrateContext _context;

        public bool Manager { get; private set; }
        public bool Director { get; private set; }
        public IEnumerable<Role> Roles { get; private set; }

        public UserGroupPositionProvider(OrchestrateContext context)
        {
            _context = context;
        }

        public async Task Initialize(int userId, int groupId)
        {
            var group = await _context.Groups.AsNoTracking()
                .Include(_ => _.Roles)
                .Include(_ => _.Directors.Where(_ => _.Id == userId))
                .Include(_ => _.Roles).ThenInclude(_ => _.Role)
                .Include(_ => _.Roles).ThenInclude(_ => _.Members)
                .FirstOrDefaultAsync(_ => _.Id == groupId);

            if (group == null) throw new ArgumentException("Group does not exist");

            Manager = group.ManagerId == userId;
            Director = group.Directors.Any();
            Roles = group.Roles.Where(_ => _.Members.Any(_ => _.Id == userId)).Select(_ => _.Role);
        }
    }
}
