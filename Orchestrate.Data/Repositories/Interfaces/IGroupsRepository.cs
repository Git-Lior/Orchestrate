using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using Orchestrate.Data.Models.Joins;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface IGroupsRepository : IEntityRepository<Group>
    {
        Task AddDirector(Group group, User director);
        Task RemoveDirector(Group group, User director);

        Task<GroupRole> AddRole(Group group, Role role);
        Task RemoveRole(Group group, int roleId);

        Task AddMember(Group group, int roleId, User member);
        Task RemoveMember(Group group, int roleId, User member);
    }
}
