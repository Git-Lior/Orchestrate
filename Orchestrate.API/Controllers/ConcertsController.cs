using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.Data.Repositories.Interfaces;
using Orchestrate.API.DTOs;
using Orchestrate.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/concerts")]
    public class ConcertsController : ApiControllerBase
    {
        private readonly IConcertsRepository _concertsRepo;

        [FromRoute]
        public int GroupId { get; set; }

        public ConcertsController(IServiceProvider provider, IConcertsRepository repository) : base(provider)
        {
            _concertsRepo = repository;
        }

        [HttpGet, ProducesOk(typeof(IEnumerable<ConcertData>))]
        [Authorize(Policy = GroupRolesPolicy.ManagerOrMember)]
        public async Task<IActionResult> GetFutureConcerts([FromQuery] bool hideNotAttending)
        {
            IQueryable<Concert> concerts = _concertsRepo.NoTrackedEntities
                .Where(_ => _.GroupId == GroupId)
                .Include(_ => _.Attendances)
                .OrderBy(_ => _.Date);

            if (UserGroupPosition.Manager)
                return Ok(await concerts.ProjectTo<ConcertDataWithUserAttendance>(MapperConfig, new { RequestingUserId }).ToListAsync());

            if (hideNotAttending) concerts = concerts.Where(_ => _.Attendances.All(_ => _.UserId != RequestingUserId || _.Attending));

            return Ok(await concerts.ProjectTo<ConcertData>(MapperConfig, new { RequestingUserId }).ToListAsync());
        }

        [HttpPost("{concertId}/attendance"), ProducesOk]
        [Authorize(Policy = GroupRolesPolicy.MemberOnly)]
        public async Task<IActionResult> SetAttendance([FromRoute] int concertId, [FromBody] bool attending)
        {
            var concert = await SingleOrError(_concertsRepo
                .FindOne(new ConcertIdentifier(GroupId, concertId))
                .Include(_ => _.Attendances.Where(a => a.UserId == RequestingUserId)));

            await _concertsRepo.SetUserAttendance(concert, RequestingUserId, attending);

            return Ok();
        }
    }
}
