using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Orchestrate.API.Data;
using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchestrate.API.Services
{
    public class UserService : IUserService
    {
        private readonly OrchestrateContext _context;
        private readonly SigningCredentials _signingCredentials;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public UserService(OrchestrateContext context, IConfiguration config)
        {
            _context = context;
            _securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config.GetValue<string>("JwtSecret")));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public async Task<UserInfo> Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(_ => _.Email == email);
            if (user == null) throw new ArgumentException("User not found");

            var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verifyResult == PasswordVerificationResult.Failed) throw new ArgumentException("Incorrect password");

            if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _hasher.HashPassword(user, password);
                await _context.SaveChangesAsync();
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.UserId.ToString()) }),
                SigningCredentials = _signingCredentials
            });

            return new UserInfo
            {
                Id = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenHandler.WriteToken(token)
            };
        }

        public UserInfo GetById(int id)
        {
            var user = _context.Users.SingleOrDefault(_ => _.UserId == id);

            return new UserInfo
            {
                Id = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
