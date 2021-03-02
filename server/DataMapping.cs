using AutoMapper;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;

namespace Orchestrate.API
{
    public class DataMapping : Profile
    {
        public DataMapping()
        {
            CreateMap<UserPayload, User>();
            CreateMap<RolePayload, Role>();
            CreateMap<GroupPayload, Group>();
            CreateMap<CompositionPayload, Composition>();

            CreateMap<User, UserData>();
            CreateMap<User, FullUserData>();
            CreateMap<User, LoggedInUserData>();
            CreateMap<User, CreatedUserData>();

            CreateMap<Group, GroupData>();
            
            CreateMap<Composition, CompositionData>();
            CreateMap<Composition, FullCompositionData>();

            CreateMap<SheetMusic, SheetMusicData>();
        }
    }
}
