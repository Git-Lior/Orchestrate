using Orchestrate.API.Models;
using Orchestrate.API.Services.Interfaces;
using System.Threading.Tasks;

namespace Orchestrate.API.Data
{
    public class OrchestrateDbInitializer
    {
        private const string TEST_USER_EMAIL = "test@test.com";
        
        private OrchestrateContext _ctx;
        private readonly IPasswordProvider _passwordProvider;

        public OrchestrateDbInitializer(OrchestrateContext ctx, IPasswordProvider passwordProvider)
        {
            _ctx = ctx;
            _passwordProvider = passwordProvider;
        }

        public async Task Initialize()
        {
            _ctx.Users.Add(new User
            {
                Email = TEST_USER_EMAIL,
                PasswordHash = _passwordProvider.HashPassword("test"),
                FirstName = "Test",
                LastName = "User",
                IsPasswordTemporary = false
            });

            await _ctx.SaveChangesAsync();
        }
    }
}
