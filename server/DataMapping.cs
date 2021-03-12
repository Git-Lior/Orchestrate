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
            CreateMap<User, UserDataWithAttendance>()
                .ForMember(_ => _.Attending, o => o.MapFrom((s, d) => s.Attendances.FirstOrDefault()?.Attending));

            CreateMap<GroupRole, BasicGroupRoleData>()
                .ForMember(_ => _.Id, o => o.MapFrom(_ => _.Role.Id))
                .ForMember(_ => _.Section, o => o.MapFrom(_ => _.Role.Section))
                .ForMember(_ => _.Num, o => o.MapFrom(_ => _.Role.Num));

            CreateMap<GroupRole, GroupRoleData>()
                .IncludeBase<GroupRole, BasicGroupRoleData>();

            CreateMap<GroupRole, GroupRoleAttendanceData>()
                .IncludeBase<GroupRole, BasicGroupRoleData>()
                .ForMember(_ => _.Attendances, o => o.MapFrom(_ => _.Members));

            CreateMap<Group, GroupData>();
            CreateMap<Group, FullGroupData>();

            CreateMap<Composition, BasicCompositionData>();
            CreateMap<Composition, CompositionData>();
            CreateMap<Composition, FullCompositionData>()
                .ForMember(f => f.Roles, o => o.MapFrom(c => c.SheetMusics.Select(s => s.Role)));

            CreateMap<SheetMusicComment, SheetMusicCommentData>();

            CreateMap<Concert, ConcertData>();
            CreateMap<Concert, ConcertDataWithAttendance>()
                .ForMember(_ => _.Attending, o => o.MapFrom(_ => _.Attendances.Where(_ => _.Attending).Select(_ => _.User)))
                .ForMember(_ => _.NotAttending, o => o.MapFrom(_ => _.Attendances.Where(_ => !_.Attending).Select(_ => _.User)));
        }
    }
}
