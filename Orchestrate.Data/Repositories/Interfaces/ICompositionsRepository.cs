using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface ICompositionsRepository : IEntityRepository<Composition>
    {
        public Task UploadSheetMusic(Composition composition, int roleId, byte[] file);
    }
}
