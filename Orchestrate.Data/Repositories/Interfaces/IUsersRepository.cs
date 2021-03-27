using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface IUsersRepository : IEntityRepository<User>
    {
        Task<User> AuthenticateUser(string email, string password);
        Task ChangePassword(User user, string oldPassword, string newPassword);
        IQueryable<User> GetUsersInGroup(int groupId);
        (string, CompleteUserPayload) GenerateNewUserPayload(UserPayload basePayload);
    }
}
