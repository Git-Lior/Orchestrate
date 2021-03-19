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

namespace Orchestrate.API.Controllers.Manager
{
    [Route("api/groups/{groupId}/concerts")]
    [Authorize(Policy = GroupRolesPolicy.ManagerOnly)]
    public class ConcertsManagerController : OrchestrateController<Concert>
    {
        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int ConcertId { get; set; }

        protected override IQueryable<Concert> MatchingEntityQuery(IQueryable<Concert> query) =>
            query.Where(_ => _.GroupId == GroupId && _.Id == ConcertId);

        public ConcertsManagerController(IServiceProvider provider) : base(provider) { }

        [HttpPost]
        public async Task<IActionResult> CreateConcert([FromBody] ConcertPayload payload)
        {
            var concert = ModelMapper.Map<Concert>(payload);
            concert.GroupId = GroupId;
            concert.CreatedAt = DateTime.Now;

            DbContext.Concerts.Add(concert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{concertId}")]
        public async Task<IActionResult> UpdateConcert([FromBody] ConcertPayload payload)
        {
            var dbConcert = await GetMatchingEntity(DbContext.Concerts);

            if (dbConcert == null) throw new ArgumentException("Concert does not exist");

            ModelMapper.Map(payload, dbConcert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{concertId}")]
        public async Task<IActionResult> DeleteConcert()
        {
            var dbConcert = await GetMatchingEntity(DbContext.Concerts);

            if (dbConcert == null) throw new ArgumentException("Concert does not exist");

            DbContext.Remove(dbConcert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{concertId}/attendance")]
        public async Task<IActionResult> GetConcertAttendance()
        {
            var groupRoles = await DbContext.Groups.Where(_ => _.Id == GroupId)
                .SelectMany(_ => _.Roles)
                    .Include(_ => _.Members)
                        .ThenInclude(_ => _.Attendances.Where(_ => _.GroupId == GroupId && _.ConcertId == ConcertId))
                .ToListAsync();

            return Ok(ModelMapper.Map<IEnumerable<GroupRoleAttendanceData>>(groupRoles));
        }

        [HttpGet("{concertId}/compositions")]
        public async Task<IActionResult> GetRelevantCompositions()
        {
            var dbConcert = await GetMatchingEntity(DbContext.Concerts);

            return Ok(await DbContext.Compositions.AsNoTracking()
                .Where(_ => _.GroupId == GroupId && _.Concerts.All(c => c.Id != ConcertId))
                .ProjectTo<CompositionData>(MapperConfig)
                .OrderBy(_ => _.Title)
                .ToListAsync());
        }

        [HttpPost("{concertId}/compositions")]
        public async Task<IActionResult> AddCompositionToConcert([FromBody] int compositionId)
        {
            var dbComposition = await GetGroupComposition(compositionId);

            var dbConcerts = await GetMatchingEntity(DbContext.Concerts
                .Include(_ => _.Compositions.Where(_ => _.GroupId == GroupId && _.Id == compositionId)));

            if (dbConcerts.Compositions.Any()) throw new ArgumentException("Composition already in concert");

            dbConcerts.Compositions.Add(dbComposition);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{concertId}/compositions/{compositionId}")]
        public async Task<IActionResult> RemoveConcertFromComposition([FromRoute] int compositionId)
        {
            var dbComposition = await GetGroupComposition(compositionId);

            var dbConcerts = await GetMatchingEntity(DbContext.Concerts
                .Include(_ => _.Compositions.Where(_ => _.GroupId == GroupId && _.Id == compositionId)));

            if (!dbConcerts.Compositions.Any()) throw new ArgumentException("Composition not in concert");

            dbConcerts.Compositions.Remove(dbComposition);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<Composition> GetGroupComposition(int compositionId)
        {
            var dbComposition = await DbContext.Compositions.FindAsync(GroupId, compositionId);

            if (dbComposition == null) throw new ArgumentException("Composition does not exist");

            return dbComposition;
        }
    }
}
