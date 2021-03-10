using AutoMapper;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System.Linq;

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

            CreateMap<GroupRole, GroupRoleData>()
                .ForMember(_ => _.Id, o => o.MapFrom(_ => _.Role.Id))
                .ForMember(_ => _.Section, o => o.MapFrom(_ => _.Role.Section))
                .ForMember(_ => _.Num, o => o.MapFrom(_ => _.Role.Num));

            CreateMap<Group, GroupData>();
            CreateMap<Group, FullGroupData>();

            CreateMap<Composition, CompositionData>();
            CreateMap<Composition, FullCompositionData>()
                .ForMember(f => f.Roles, o => o.MapFrom(c => c.SheetMusics.Select(s => s.Role)));

            CreateMap<SheetMusicComment, SheetMusicCommentData>();
        }
    }
}
