using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Data.Repositories
{
    public class RolesRepository : EntityRepositoryBase<Role>
    {
        protected override DbSet<Role> DbEntities => Context.Roles;
        public RolesRepository(OrchestrateContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<Role> FindOne(object identifier)
        {
            if (identifier is not RoleIdentifier r) throw new InvalidOperationException("invalid role identifier");

            return DbEntities.Where(_ => _.Id == r.RoleId);
        }

        public async Task<Role> GetOrCreate(string section, int? num)
        {
            var dbRole = await DbEntities.Where(_ => _.Section == section && _.Num == num).SingleOrDefaultAsync();

            if (dbRole != null) return dbRole;

            return await Create(new RolePayload { Section = section, Num = num });
        }
    }
}
