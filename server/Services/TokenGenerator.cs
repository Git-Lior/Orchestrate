using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orchestrate.API.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Orchestrate.API.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly SigningCredentials _signingCredentials;

        public TokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtOptions.Value.JwtSecretBytes), SecurityAlgorithms.HmacSha256Signature);
        }

        public string GenerateUserToken(int userId)
        {
            return GenerateToken(new Claim[] { new Claim(ClaimTypes.Name, userId.ToString()) });
        }

        public string GenerateAdminToken()
        {
            return GenerateToken(new Claim[] { new Claim(ClaimTypes.Role, "Administrator") });
        }

        private string GenerateToken(Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = _signingCredentials
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
