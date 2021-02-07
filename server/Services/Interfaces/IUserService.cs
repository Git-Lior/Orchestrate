using System.Threading.Tasks;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserInfo> Authenticate(string email, string password);
        UserInfo GetById(int userId);
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
    }
}
