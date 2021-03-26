using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.Data.Repositories;
using Orchestrate.API.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers.Director
{
    [Route("api/groups/{groupId}/compositions")]
    [Authorize(Policy = GroupRolesPolicy.DirectorOnly)]
    public class CompositionDirectorController : ApiControllerBase
    {
        private readonly CompositionsRepository _compositionsRepo;

        [FromRoute]
        public int GroupId { get; set; }
        [FromRoute]
        public int CompositionId { get; set; }

        public CompositionIdentifier EntityId => new CompositionIdentifier(GroupId, CompositionId);

        public CompositionDirectorController(IServiceProvider provider, CompositionsRepository repository) : base(provider)
        {
            _compositionsRepo = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddComposition([FromBody] CompleteCompositionPayload payload)
        {
            payload.GroupId = GroupId;
            payload.UploaderId = RequestingUserId;
            payload.CreatedAt = DateTime.UtcNow;

            await _compositionsRepo.Create(payload);

            return Ok();
        }

        [HttpPut("{compositionId}")]
        public async Task<IActionResult> UpdateComposition([FromBody] CompositionPayload payload)
        {
            var composition = await SingleOrError(_compositionsRepo.FindOne(EntityId));
            await _compositionsRepo.Update(composition, payload);

            return Ok();
        }

        [HttpDelete("{compositionId}")]
        public async Task<IActionResult> RemoveComposition()
        {
            var composition = await SingleOrError(_compositionsRepo.FindOne(EntityId));
            await _compositionsRepo.Delete(composition);

            return Ok();
        }

        [HttpPost("{compositionId}/{roleId}")]
        public async Task<IActionResult> UploadSheetMusicFile([FromRoute] int roleId, IFormFile file)
        {
            var composition = await SingleOrError(_compositionsRepo.FindOne(EntityId));

            using var stream = new MemoryStream((int)file.Length);
            await file.CopyToAsync(stream);

            await _compositionsRepo.UploadSheetMusic(composition, roleId, stream.ToArray());

            return Ok();
        }
    }
}
