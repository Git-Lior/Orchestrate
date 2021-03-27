using AutoMapper;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Models;
using System;
using System.Linq;

namespace Orchestrate.API
{
    public class DataMapping : Profile
    {
        public DataMapping()
        {
            int RequestingUserId = 0; // this will be passd at runtime

            CreateMap<DateTimeOffset, long>().ConvertUsing(d => d.ToUnixTimeSeconds());

            CreateMap<UserPayload, CompleteUserPayload>();

            CreateMap<UserPayload, User>();
            CreateMap<CompleteUserPayload, User>();
            CreateMap<RolePayload, Role>();
            CreateMap<GroupPayload, Group>();
            CreateMap<CompositionPayload, Composition>();
            CreateMap<CompleteCompositionPayload, Composition>();
            CreateMap<ConcertPayload, Concert>();
            CreateMap<CompleteConcertPayload, Concert>();

            CreateMap<User, UserData>();
            CreateMap<User, FullUserData>();
            CreateMap<User, LoggedInUserData>();
            CreateMap<User, CreatedUserData>();
            CreateMap<User, UserDataWithAttendance>()
                .ForMember(_ => _.Attending, o => o.MapFrom(s => s.Attendances.FirstOrDefault().Attending));

            CreateMap<Role, RoleData>();

            CreateMap<GroupRole, RoleData>()
                .ForMember(_ => _.Id, o => o.MapFrom(_ => _.Role.Id))
                .ForMember(_ => _.Section, o => o.MapFrom(_ => _.Role.Section))
                .ForMember(_ => _.Num, o => o.MapFrom(_ => _.Role.Num));

            CreateMap<GroupRole, GroupRoleData>()
                .IncludeBase<GroupRole, RoleData>();

            CreateMap<GroupRole, GroupRoleAttendanceData>()
                .IncludeBase<GroupRole, RoleData>()
                .ForMember(_ => _.Attendances, o => o.MapFrom(_ => _.Members));

            CreateMap<Group, GroupData>();
            CreateMap<Group, FullGroupData>();

            CreateMap<Composition, BasicCompositionData>();
            CreateMap<Composition, CompositionData>();
            CreateMap<Composition, FullCompositionData>()
                .ForMember(f => f.Roles, o => o.MapFrom(c => c.SheetMusics.Select(s => s.Role)));
            CreateMap<Composition, CompositionUpdateData>()
                .ForMember(_ => _.Date, o => o.MapFrom(_ => _.CreatedAt));

            CreateMap<SheetMusicComment, SheetMusicCommentData>();

            CreateMap<Concert, BasicConcertData>();
            CreateMap<Concert, ConcertData>()
                .ForMember(_ => _.Attending, o => o.MapFrom(_ => _.Attendances.FirstOrDefault(a => a.UserId == RequestingUserId).Attending));

            CreateMap<Concert, ConcertDataWithUserAttendance>()
                .IncludeBase<Concert, ConcertData>()
                .ForMember(_ => _.AttendingUsers, o => o.MapFrom(_ => _.Attendances.Where(_ => _.Attending).Select(_ => _.User)))
                .ForMember(_ => _.NotAttendingUsers, o => o.MapFrom(_ => _.Attendances.Where(_ => !_.Attending).Select(_ => _.User)));
        }
    }
}
