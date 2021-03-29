using Orchestrate.Data.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Orchestrate.Data
{
    public class StringHasher : IStringHasher
    {
        private readonly int _hashIterations;

        public StringHasher(int hashIterations)
        {
            _hashIterations = hashIterations;
        }

        public string Hash(string input)
        {
            using var algorithm = new Rfc2898DeriveBytes(input, 16, 10000, HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(32));
            var salt = Convert.ToBase64String(algorithm.Salt);
            return $"{_hashIterations}.{salt}.{key}";
        }

        public (bool success, bool needsUpgrade) CheckHash(string hash, string input)
        {
            var parts = hash.Split(".", 3);
            if (parts.Length != 3) throw new Exception("Unexpected hash format");

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(input, salt, iterations, HashAlgorithmName.SHA256);
            var verified = algorithm.GetBytes(32).SequenceEqual(key);

            return (verified, iterations != _hashIterations);
        }
    }
}
