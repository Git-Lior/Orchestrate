using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;
using System.Threading.Tasks;

namespace Orchestrate.Data.Repositories.Interfaces
{
    public interface ISheetMusicsRepository : IEntityRepository<SheetMusic>
    {
        Task AddComment(SheetMusic sheetMusic, int userId, string content);
        Task UpdateComment(SheetMusic sheetMusic, int userId, int commentId, string content);
        Task DeleteComment(SheetMusic sheetMusic, int userId, int commentId);
    }
}
