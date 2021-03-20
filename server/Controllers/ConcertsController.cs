using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/concerts")]
    public class ConcertsController : OrchestrateController<Concert>
    {
        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int ConcertId { get; set; }

        protected override string EntityName => "Concert";
        protected override IQueryable<Concert> MatchingEntityQuery(IQueryable<Concert> query) =>
            query.Where(_ => _.GroupId == GroupId && _.Id == ConcertId);

        public ConcertsController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        [Authorize(Policy = GroupRolesPolicy.ManagerOrMember)]
        public async Task<IActionResult> GetFutureConcerts([FromQuery] bool hideNotAttending)
        {
            IQueryable<Concert> concerts = DbContext.Concerts.AsNoTracking()
                .Where(_ => _.GroupId == GroupId)
                .Include(_ => _.Attendances)
                .OrderBy(_ => _.Date);

            if (IsUserManager)
                return Ok(await concerts.ProjectTo<ConcertDataWithUserAttendance>(MapperConfig, new { RequestingUserId }).ToListAsync());

            if (hideNotAttending) concerts = concerts.Where(_ => _.Attendances.All(_ => _.UserId != RequestingUserId || _.Attending));

            return Ok(await concerts.ProjectTo<ConcertData>(MapperConfig, new { RequestingUserId }).ToListAsync());
        }

        [HttpPost("{concertId}/attendance")]
        [Authorize(Policy = GroupRolesPolicy.MemberOnly)]
        public async Task<IActionResult> SetAttendance([FromBody] bool attending)
        {
            var concert = await GetMatchingEntity(DbContext.Concerts.Include(_ => _.Attendances.Where(a => a.UserId == RequestingUserId)));

            var attendance = concert.Attendances.FirstOrDefault();

            if (attendance == null)
                concert.Attendances.Add(new ConcertAttendance { UserId = RequestingUserId, UpdatedAt = DateTime.UtcNow, Attending = attending });
            else
            {
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.Attending = attending;
            }

            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
