using Orchestrate.API.Models;
using System.Threading.Tasks;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string email, string password);
    }
}
