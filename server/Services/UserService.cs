using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrate.API.Services
{
    public class UserService : IUserService
    {
        private readonly OrchestrateContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly int _hashIterations;

        public UserService(OrchestrateContext context, IOptions<PasswordHashOptions> hashOptions, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _hashIterations = hashOptions.Value.HashIterations;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<UserData> Authenticate(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(_ => _.Email == email);
            if (user == null) throw new ArgumentException("User not found");

            var (success, needsUpgrade) = CheckHash(user.PasswordHash, password);

            if (!success) throw new ArgumentException("Incorrect password");

            if (needsUpgrade)
            {
                user.PasswordHash = GenerateHash(password);
                await _context.SaveChangesAsync();
            }

            return new UserData
            {
                Id = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsPasswordTemporary = user.IsPasswordTemporary,
                Token = _tokenGenerator.GenerateUserToken(user.UserId)
            };
        }

        public async Task ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await FindUser(userId);

            var (success, _) = CheckHash(user.PasswordHash, oldPassword);

            if (!success) throw new ArgumentException("Incorrect password");

            var newHash = GenerateHash(newPassword);
            user.PasswordHash = newHash;

            await _context.SaveChangesAsync();
        }

        public Task<List<UserData>> GetAll()
        {
            return _context.Users.Select(u => new UserData
            {
                Id = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToListAsync();
        }

        public async Task<UserData> GetById(int userId)
        {
            var user = await FindUser(userId);

            return new UserData
            {
                Id = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsPasswordTemporary = user.IsPasswordTemporary
            };
        }

        public async Task<UserData> Create(UserData data)
        {
            var tempPassword = GenerateTemporaryPassword(16);

            var result = _context.Users.Add(new User
            {
                Email = data.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
                PasswordHash = GenerateHash(tempPassword),
                IsPasswordTemporary = true
            });

            await _context.SaveChangesAsync();

            return new UserData
            {
                Id = result.Entity.UserId,
                Email = result.Entity.Email,
                FirstName = result.Entity.FirstName,
                LastName = result.Entity.LastName,
                IsPasswordTemporary = true,
                TemporaryPassword = tempPassword
            };
        }

        public async Task Update(int userId, UserData data)
        {
            var user = await FindUser(userId);

            user.Email = data.Email;
            user.FirstName = data.FirstName;
            user.LastName = data.LastName;

            await _context.SaveChangesAsync();
        }

        public Task Delete(int userId)
        {
            _context.Users.Remove(new User { UserId = userId });
            return _context.SaveChangesAsync();
        }

        private async Task<User> FindUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");
            
            return user;
        }

        private string GenerateHash(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(32));
            var salt = Convert.ToBase64String(algorithm.Salt);
            return $"{_hashIterations}.{salt}.{key}";
        }

        private (bool verified, bool needsUpgrade) CheckHash(string hash, string password)
        {
            var parts = hash.Split(".", 3);
            if (parts.Length != 3) throw new Exception("Unexpected hash format");

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var verified = algorithm.GetBytes(32).SequenceEqual(key);

            return (verified, iterations != _hashIterations);
        }

        private string GenerateTemporaryPassword(int size)
        {
            var rnd = new Random();
            var sb = new StringBuilder(size);

            for (int i = 0; i < size; i++) sb.Append((char)rnd.Next(65, 123));

            return sb.ToString();
        }
    }
}
