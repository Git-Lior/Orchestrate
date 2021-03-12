using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
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

        protected override IQueryable<Concert> MatchingEntityQuery(IQueryable<Concert> query) =>
            query.Where(_ => _.GroupId == GroupId && _.Id == ConcertId);

        public ConcertsController(IServiceProvider provider) : base(provider) { }

        [HttpGet]
        [Authorize(Policy = GroupRolesPolicy.ManagerOrMember)]
        public async Task<IActionResult> GetConcerts([FromQuery] DateTime? until, [FromQuery] int limit, [FromQuery] bool onlyAttending)
        {
            IQueryable<Concert> concerts = DbContext.Concerts.AsNoTracking()
                .Where(_ => _.GroupId == GroupId)
                .OrderByDescending(_ => _.Date);

            if (until != null) concerts = concerts.Where(_ => _.Date < until);
            if (limit > 0) concerts = concerts.Take(limit);

            if (IsUserManager)
            {
                var result = await concerts.Include(_ => _.Attendances).ToListAsync();
                return Ok(ModelMapper.Map<IEnumerable<ConcertDataWithAttendance>>(result));
            }

            if (onlyAttending) concerts = concerts.Where(_ => _.Attendances.Any(_ => _.UserId == RequestingUserId && _.Attending));

            return Ok(await concerts.ProjectTo<ConcertData>(MapperConfig).ToListAsync());
        }

        [HttpPost("attendance")]
        [Authorize(Policy = GroupRolesPolicy.MemberOnly)]
        public async Task<IActionResult> SetAttendance([FromBody] bool attending)
        {
            var concert = await GetMatchingEntity(DbContext.Concerts.Include(_ => _.Attendances.Where(a => a.UserId == RequestingUserId)));

            if (concert == null) throw new ArgumentException("Concert does not exist");

            var attendance = concert.Attendances.FirstOrDefault();

            if (attendance == null)
                concert.Attendances.Add(new ConcertAttendance { UserId = RequestingUserId, Attending = attending });
            else
                attendance.Attending = attending;

            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
