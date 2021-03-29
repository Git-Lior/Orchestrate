using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface IUsersRepository : IEntityRepository<User>
    {
        IQueryable<User> GetUsersInGroup(int groupId);
        Task<(string temporaryPassword, User user)> CreateNewUser(UserPayload payload);
        Task<User> AuthenticateUser(string email, string password);
        Task ChangePassword(User user, string oldPassword, string newPassword);
    }
}
