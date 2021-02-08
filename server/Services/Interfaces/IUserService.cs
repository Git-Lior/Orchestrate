using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserData> Authenticate(string email, string password);
        Task ChangePassword(int userId, string oldPassword, string newPassword);

        Task<List<UserData>> GetAll();
        Task<UserData> GetById(int userId);
        Task<UserData> Create(UserData data);
        Task Update(int userId, UserData data);
        Task Delete(int userId);
    }

    public class UserData
    {
        public int? Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Token { get; set; }
        public bool? IsPasswordTemporary { get; set; }
        public string? TemporaryPassword { get; set; }
    }
}
