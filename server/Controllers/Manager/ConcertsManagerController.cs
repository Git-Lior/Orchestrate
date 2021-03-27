using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.Data.Repositories.Interfaces;
using Orchestrate.Data.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Manager
{
    [Route("api/groups/{groupId}/concerts")]
    [Authorize(Policy = GroupRolesPolicy.ManagerOnly)]
    public class ConcertsManagerController : ApiControllerBase
    {
        private IConcertsRepository _concertsRepo;

        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int ConcertId { get; set; }

        public ConcertIdentifier EntityId => new ConcertIdentifier(GroupId, ConcertId);

        public ConcertsManagerController(IServiceProvider provider, IConcertsRepository repository) : base(provider)
        {
            _concertsRepo = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateConcert([FromBody] CompleteConcertPayload payload)
        {
            payload.GroupId = GroupId;
            payload.CreatedAt = DateTime.UtcNow;

            await _concertsRepo.Create(payload);

            return Ok();
        }

        [HttpPut("{concertId}")]
        public async Task<IActionResult> UpdateConcert([FromBody] ConcertPayload payload)
        {
            var concert = await SingleOrError(_concertsRepo.FindOne(EntityId));
            await _concertsRepo.Update(concert, payload);

            return Ok();
        }

        [HttpDelete("{concertId}")]
        public async Task<IActionResult> DeleteConcert()
        {
            var concert = await SingleOrError(_concertsRepo.FindOne(EntityId));
            await _concertsRepo.Delete(concert);

            return Ok();
        }

        [HttpPost("{concertId}/compositions")]
        public async Task<IActionResult> AddCompositionToConcert([FromBody] int compositionId)
        {
            var concert = await SingleOrError(_concertsRepo
                .FindOne(EntityId)
                .Include(_ => _.Compositions));

            var composition = await SingleOrError(Repository.Get<Composition>().FindOne(new CompositionIdentifier(GroupId, compositionId)));

            await _concertsRepo.AddComposition(concert, composition);

            return Ok();
        }

        [HttpDelete("{concertId}/compositions/{compositionId}")]
        public async Task<IActionResult> RemoveConcertFromComposition([FromRoute] int compositionId)
        {
            var concert = await SingleOrError(_concertsRepo.FindOne(EntityId).Include(_ => _.Compositions));

            var composition = await SingleOrError(Repository.Get<Composition>().FindOne(new CompositionIdentifier(GroupId, compositionId)));

            await _concertsRepo.RemoveComposition(concert, composition);

            return Ok();
        }
    }
}
