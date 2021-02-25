using AutoMapper;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;

namespace Orchestrate.API
{
    public class DataMapping : Profile
    {
        public DataMapping()
        {
            CreateMap<User, UserData>();
            CreateMap<User, UserDataWithToken>();
            CreateMap<User, UserDataWithTemporaryPassword>();

            CreateMap<Group, GroupData>();

            CreateMap<Composition, CompositionData>();
            CreateMap<Composition, FullCompositionData>();

            CreateMap<SheetMusic, SheetMusicData>();
        }
    }
}
