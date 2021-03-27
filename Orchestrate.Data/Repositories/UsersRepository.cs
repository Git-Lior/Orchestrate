using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Models;
using Orchestrate.Data.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orchestrate.Data.Repositories.Interfaces;

namespace Orchestrate.Data.Repositories
{
    public class UsersRepository : EntityRepositoryBase<User>, IUsersRepository
    {
        private readonly IPasswordProvider _passwordProvider;

        protected override DbSet<User> DbEntities => Context.Users;

        public UsersRepository(OrchestrateContext context,
                               IMapper mapper,
                               IPasswordProvider passwordProvider) : base(context, mapper)
        {
            _passwordProvider = passwordProvider;
        }

        public override IQueryable<User> FindOne(object identifier)
        {
            if (identifier is not UserIdentifier u) throw new InvalidOperationException("invalid user identifier");

            return DbEntities.Where(_ => _.Id == u.UserId);
        }

        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await DbEntities.FirstOrDefaultAsync(_ => _.Email == email);
            if (user == null) throw new ArgumentException("User not found");

            var (success, needsUpgrade) = _passwordProvider.CheckHash(user.PasswordHash, password);

            if (!success) throw new ArgumentException("Incorrect password");

            if (needsUpgrade)
            {
                user.PasswordHash = _passwordProvider.HashPassword(password);
                await Context.SaveChangesAsync();
            }

            return user;
        }

        public async Task ChangePassword(User user, string oldPassword, string newPassword)
        {
            var (success, _) = _passwordProvider.CheckHash(user.PasswordHash, oldPassword);
            if (!success) throw new ArgumentException("Incorrect Password");

            user.PasswordHash = _passwordProvider.HashPassword(newPassword);
            user.IsPasswordTemporary = false;

            await Context.SaveChangesAsync();
        }

        public IQueryable<User> GetUsersInGroup(int groupId)
        {
            if (groupId == 0) return NoTrackedEntities;
            return NoTrackedEntities.Where(u => u.ManagingGroups.Any(g => g.Id == groupId)
                                             || u.DirectorOfGroups.Any(g => g.Id == groupId)
                                             || u.MemberOfGroups.Any(g => g.GroupId == groupId));
        }

        public (string, CompleteUserPayload) GenerateNewUserPayload(UserPayload basePayload)
        {
            var payload = Mapper.Map<CompleteUserPayload>(basePayload);

            string password = _passwordProvider.GenerateTemporaryPassword(16);
            payload.PasswordHash = _passwordProvider.HashPassword(password);
            payload.IsPasswordTemporary = true;

            return (password, payload);
        }

        public override async Task Delete(User user)
        {
            if (await Context.Groups.AnyAsync(_ => _.ManagerId == user.Id))
                throw new ArgumentException("Cannot delete a user that manages a group, change the group's manager first");

            await base.Delete(user);
        }
    }
}
