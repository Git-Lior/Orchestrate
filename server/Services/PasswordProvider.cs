using Microsoft.Extensions.Options;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Orchestrate.API.Services
{
    public class PasswordProvider : IPasswordProvider
    {
        private readonly int _hashIterations;

        public PasswordProvider(IOptions<PasswordHashOptions> hashOptions)
        {
            _hashIterations = hashOptions.Value.HashIterations;
        }

        public string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(32));
            var salt = Convert.ToBase64String(algorithm.Salt);
            return $"{_hashIterations}.{salt}.{key}";
        }

        public (bool success, bool needsUpgrade) CheckHash(string hash, string password)
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

        public string GenerateTemporaryPassword(int size)
        {
            var rnd = new Random();
            var sb = new StringBuilder(size);

            for (int i = 0; i < size; i++) sb.Append((char)rnd.Next(65, 123));

            return sb.ToString();
        }
    }
}
