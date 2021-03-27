using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface IConcertsRepository : IEntityRepository<Concert>
    {
        public Task AddComposition(Concert concert, Composition composition);
        public Task RemoveComposition(Concert concert, Composition composition);
        public Task SetUserAttendance(Concert concert, int userId, bool attending);
    }
}
