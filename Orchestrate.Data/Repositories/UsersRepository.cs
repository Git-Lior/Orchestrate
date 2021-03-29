using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Models;
using Orchestrate.Data.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orchestrate.Data.Repositories.Interfaces;
using System.Text;

namespace Orchestrate.Data.Repositories
{
    public class UsersRepository : EntityRepositoryBase<User>, IUsersRepository
    {
        private readonly IStringHasher _stringHasher;

        protected override DbSet<User> DbEntities => Context.Users;

        public UsersRepository(OrchestrateContext context,
                               IMapper mapper,
                               IStringHasher stringHasher) : base(context, mapper)
        {
            _stringHasher = stringHasher;
        }

        public override IQueryable<User> FindOne(object identifier)
        {
            if (identifier is not UserIdentifier u) throw new InvalidOperationException("invalid user identifier");

            return DbEntities.Where(_ => _.Id == u.UserId);
        }

        public IQueryable<User> GetUsersInGroup(int groupId)
        {
            if (groupId == 0) return NoTrackedEntities;
            return NoTrackedEntities.Where(u => u.ManagingGroups.Any(g => g.Id == groupId)
                                             || u.DirectorOfGroups.Any(g => g.Id == groupId)
                                             || u.MemberOfGroups.Any(g => g.GroupId == groupId));
        }

        public async Task<(string, User)> CreateNewUser(UserPayload basePayload)
        {
            var payload = Mapper.Map<UserFields>(basePayload);

            string password = GenerateTemporaryPassword(16);
            payload.PasswordHash = _stringHasher.Hash(password);
            payload.IsPasswordTemporary = true;

            var result = await base.Create(payload);

            return (password, result);
        }

        public override Task<User> Create(object payload) =>
            throw new InvalidOperationException("Cannot create new user directly, use CreateNewUser function");

        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await DbEntities.FirstOrDefaultAsync(_ => _.Email == email);
            if (user == null) throw new ArgumentException("User not found");

            var (success, needsUpgrade) = _stringHasher.CheckHash(user.PasswordHash, password);

            if (!success) throw new ArgumentException("Incorrect password");

            if (needsUpgrade)
            {
                user.PasswordHash = _stringHasher.Hash(password);
                await Context.SaveChangesAsync();
            }

            return user;
        }

        public async Task ChangePassword(User user, string oldPassword, string newPassword)
        {
            var (success, _) = _stringHasher.CheckHash(user.PasswordHash, oldPassword);
            if (!success) throw new ArgumentException("Incorrect Password");

            user.PasswordHash = _stringHasher.Hash(newPassword);
            user.IsPasswordTemporary = false;

            await Context.SaveChangesAsync();
        }

        public override async Task Delete(User user)
        {
            if (await Context.Groups.AnyAsync(_ => _.ManagerId == user.Id))
                throw new ArgumentException("Cannot delete a user that manages a group, change the group's manager first");

            await base.Delete(user);
        }

        private string GenerateTemporaryPassword(int size)
        {
            var rnd = new Random();
            var sb = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                int rndInt = rnd.Next(0, 52);
                int letterCode = rndInt + (rndInt / 26) * 6;
                sb.Append((char)(letterCode + 65));
            }

            return sb.ToString();
        }
    }
}
