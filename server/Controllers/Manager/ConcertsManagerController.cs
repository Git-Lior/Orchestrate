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

namespace Orchestrate.API.Controllers.Manager
{
    [Route("api/groups/{groupId}/concerts")]
    [Authorize(Policy = GroupRolesPolicy.ManagerOnly)]
    public class ConcertsManagerController : EntityApiControllerBase<Concert>
    {
        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int ConcertId { get; set; }

        protected override string EntityName => "Concert";
        protected override IQueryable<Concert> MatchingEntityQuery(IQueryable<Concert> query) =>
            query.Where(_ => _.GroupId == GroupId && _.Id == ConcertId);

        public ConcertsManagerController(IServiceProvider provider) : base(provider) { }

        [HttpPost]
        public async Task<IActionResult> CreateConcert([FromBody] ConcertPayload payload)
        {
            var concert = ModelMapper.Map<Concert>(payload);
            concert.GroupId = GroupId;
            concert.CreatedAt = DateTime.UtcNow;

            DbContext.Concerts.Add(concert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{concertId}")]
        public async Task<IActionResult> UpdateConcert([FromBody] ConcertPayload payload)
        {
            var dbConcert = await GetMatchingEntity(DbContext.Concerts);

            ModelMapper.Map(payload, dbConcert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{concertId}")]
        public async Task<IActionResult> DeleteConcert()
        {
            var dbConcert = await GetMatchingEntity(DbContext.Concerts);

            DbContext.Remove(dbConcert);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{concertId}/compositions")]
        public async Task<IActionResult> GetRelevantCompositions()
        {
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

            var dbConcert = await GetMatchingEntity(DbContext.Concerts
                .Include(_ => _.Compositions.Where(_ => _.GroupId == GroupId && _.Id == compositionId)));

            if (dbConcert.Compositions.Any()) throw new ArgumentException("Composition already in concert");

            dbConcert.Compositions.Add(dbComposition);

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
