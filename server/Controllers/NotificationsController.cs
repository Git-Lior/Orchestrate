using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
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
            var relevanceDate = new DateTime(
                Math.Min(
                    Math.Max(
                        lastUpdate?.Ticks ?? 0,
                        DateTime.Today.AddDays(-7).Ticks
                    ),
                    DateTime.Today.AddDays(-1).Ticks
                )
            );

            var sheetMusics = await DbContext.SheetMusics
                .Where(_ => _.Group.Directors.Any(_ => _.Id == RequestingUserId))
                .Union(DbContext.Users
                        .Where(_ => _.Id == RequestingUserId)
                        .SelectMany(_ => _.MemberOfGroups)
                        .SelectMany(_ => _.SheetMusics))
                .Include(_ => _.Comments.Where(_ => _.CreatedAt > relevanceDate && _.UserId != RequestingUserId && _.Content != null))
                .Include(_ => _.Composition)
                .Include(_ => _.Role)
                .ToListAsync();

            var concerts = await DbContext.Concerts
                .Where(c => c.Group.ManagerId == RequestingUserId)
                .Union(DbContext.Concerts
                    .Where(_ => _.Attendances.Any(_ => _.UserId == RequestingUserId && _.Attending)))
                .Where(c => c.Date > DateTime.UtcNow && c.Date < DateTime.Today.AddDays(1))
                .ToListAsync();

            IEnumerable<dynamic> sheetMusicNotifications = sheetMusics.Where(_ => _.Comments.Count > 0)
                .Select(s => new SheetMusicNotificationData
                {
                    Date = ModelMapper.Map<long>(s.Comments.Max(_ => _.CreatedAt)),
                    GroupId = s.GroupId,
                    Composition = ModelMapper.Map<BasicCompositionData>(s.Composition),
                    Role = ModelMapper.Map<BasicGroupRoleData>(s.Role),
                    Comments = s.Comments.Count
                });

            var concertNotifications = concerts.Select(c => new ConcertNotificationData
            {
                Date = ModelMapper.Map<long>((DateTimeOffset)DateTime.Today.ToUniversalTime()),
                GroupId = c.GroupId,
                Concert = ModelMapper.Map<BasicConcertData>(c)
            });

            return Ok(concertNotifications.Concat(sheetMusicNotifications).OrderByDescending(_ => _.Date));
        }
    }
}
