using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Data.Repositories
{
    public class GroupsRepository : EntityRepositoryBase<Group>
    {
        protected override DbSet<Group> DbEntities => Context.Groups;

        public GroupsRepository(OrchestrateContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<Group> FindOne(object identifier)
        {
            if (identifier is not GroupIdentifier g) throw new InvalidOperationException("invalid group identifier");

            return DbEntities.Where(_ => _.Id == g.GroupId);
        }

        public async Task AddDirector(Group group, User director)
        {
            if (group.Directors.Any(_ => _.Id == director.Id))
                throw new ArgumentException("Director already in group");

            group.Directors.Add(director);
            await Context.SaveChangesAsync();
        }

        public async Task RemoveDirector(Group group, User director)
        {
            if (group.Directors.All(_ => _.Id != director.Id))
                throw new ArgumentException("Director does not exist in group");

            group.Directors.Remove(director);
            await Context.SaveChangesAsync();
        }

        public async Task<GroupRole> AddRole(Group group, Role role)
        {
            if (group.Roles.Any(_ => _.RoleId == role.Id))
                throw new ArgumentException("Role already exists in group");

            var groupRole = new GroupRole { Role = role };
            group.Roles.Add(groupRole);

            await Context.SaveChangesAsync();

            return groupRole;
        }

        public async Task RemoveRole(Group group, int roleId)
        {
            var groupRole = GetGroupRole(group, roleId);

            var groupCount = await DbEntities.Where(_ => _.Roles.Any(_ => _.RoleId == roleId)).CountAsync();

            if (groupCount > 1) Context.Remove(groupRole);
            else
            {
                var role = await Context.Roles.FindAsync(roleId);
                Context.Roles.Remove(role);
            }

            await Context.SaveChangesAsync();
        }

        public async Task AddMember(Group group, int roleId, User member)
        {
            var groupRole = GetGroupRole(group, roleId);

            if (groupRole.Members.Contains(member))
                throw new ArgumentException("Member already exists in role");

            groupRole.Members.Add(member);
            await Context.SaveChangesAsync();
        }

        public async Task RemoveMember(Group group, int roleId, User member)
        {
            var groupRole = GetGroupRole(group, roleId);

            if (!groupRole.Members.Contains(member))
                throw new ArgumentException("Member doesn't exist in this role");

            groupRole.Members.Remove(member);

            await Context.SaveChangesAsync();
        }

        private GroupRole GetGroupRole(Group group, int roleId)
        {
            var groupRole = group.Roles.SingleOrDefault(_ => _.RoleId == roleId);

            if (groupRole == null) throw new ArgumentException("Role does not exist in group");

            return groupRole;
        }
    }
}
