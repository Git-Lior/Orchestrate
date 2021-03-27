using System.Text;

namespace Orchestrate.API
{
    public class AdminOptions
    {
        public string AdminPassword { get; set; }
    }

    public class JwtOptions
    {
        public string AdminRoleName { get; set; }
        public string JwtSecret { get; set; }
        public byte[] JwtSecretBytes => Encoding.UTF8.GetBytes(JwtSecret);
    }

    public class PasswordHashOptions
    {
        public int HashIterations { get; set; }
    }
}
