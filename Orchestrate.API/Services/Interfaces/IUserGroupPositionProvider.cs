using Orchestrate.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchestrate.API.Services.Interfaces
{
    public interface IUserGroupPositionProvider
    {
        bool Manager { get; }
        bool Director { get; }
        IEnumerable<Role> Roles { get; }

        Task Initialize(int userId, int groupId);
    }
}
