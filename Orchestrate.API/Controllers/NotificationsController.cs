using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    public class NotificationsController : ApiControllerBase
    {
        public NotificationsController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] DateTime? lastUpdate)
        {
            var userRoles = Repository.Get<User>().Entities.Where(_ => _.Id == RequestingUserId).SelectMany(_ => _.MemberOfGroups);

            var relevanceDate = new DateTime(
                Math.Min(
                    Math.Max(
                        lastUpdate?.Ticks ?? 0,
                        DateTime.Today.AddDays(-7).Ticks
                    ),
                    DateTime.Today.AddDays(-1).Ticks
                )
            );

            var sheetMusics = await Repository.Get<SheetMusic>().Entities
                .Where(_ => _.Group.Directors.Any(_ => _.Id == RequestingUserId))
                .Union(userRoles.SelectMany(_ => _.SheetMusics))
                .Include(_ => _.Comments.Where(_ => _.CreatedAt > relevanceDate && _.UserId != RequestingUserId && _.Content != null))
                .Include(_ => _.Composition)
                .Include(_ => _.Role)
                .ToListAsync();

            var upcomingConcerts = await Repository.Get<Concert>().Entities
                .Where(c => c.Group.ManagerId == RequestingUserId || c.Attendances.Any(_ => _.UserId == RequestingUserId && _.Attending))
                .Where(c => c.Date > DateTime.UtcNow && c.Date < DateTime.Today.AddDays(1))
                .ToListAsync();

            var newConcerts = await userRoles.SelectMany(_ => _.Group.Concerts)
                .Where(_ => _.CreatedAt > relevanceDate)
                .ToListAsync();

            IEnumerable<dynamic> sheetMusicNotifications = sheetMusics.Where(_ => _.Comments.Count > 0)
                .Select(s => new SheetMusicNotificationData
                {
                    Date = Mapper.Map<long>(s.Comments.Max(_ => _.CreatedAt)),
                    GroupId = s.GroupId,
                    Composition = Mapper.Map<BasicCompositionData>(s.Composition),
                    Role = Mapper.Map<RoleData>(s.Role),
                    Comments = s.Comments.Count
                });

            var concertNotifications = upcomingConcerts.Concat(newConcerts)
                .Select(c => new ConcertNotificationData
                {
                    Date = Mapper.Map<long>((DateTimeOffset)c.Date.Date),
                    GroupId = c.GroupId,
                    Concert = Mapper.Map<BasicConcertData>(c)
                });

            return Ok(concertNotifications.Concat(sheetMusicNotifications).OrderByDescending(_ => _.Date));
        }
    }
}
