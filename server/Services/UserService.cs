using Microsoft.AspNetCore.Identity;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Services
{
    public class UserService : IUserService
    {
        private readonly OrchestrateContext _context;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public UserService(OrchestrateContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(_ => _.Email == email);
            if (user == null) throw new ArgumentException("user not found");

            var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verifyResult == PasswordVerificationResult.Failed) throw new ArgumentException("incorrect password");

            if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _hasher.HashPassword(user, password);
                await _context.SaveChangesAsync();
            }

            return user;
        }
    }
}
