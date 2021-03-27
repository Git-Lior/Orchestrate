using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface IRolesRepository : IEntityRepository<Role>
    {
        Task<Role> GetOrCreate(string section, int? num);
    }
}
