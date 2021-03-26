using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using Orchestrate.API.Controllers.Helpers;
using Orchestrate.API.Data.Repositories;
using Orchestrate.API.DTOs;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/groups/{groupId}/compositions")]
    public class CompositionsController : ApiControllerBase
    {
        private readonly CompositionsRepository _compositionsRepo;

        [FromRoute]
        public int GroupId { get; set; }

        public CompositionsController(IServiceProvider provider, CompositionsRepository repository) : base(provider)
        {
            _compositionsRepo = repository;
        }

        [HttpGet("genres")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetGenres()
        {
            return Ok(await _compositionsRepo.NoTrackedEntities
                .Where(_ => _.GroupId == GroupId)
                .Select(_ => _.Genre).Distinct()
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCompositions([FromQuery] string title, [FromQuery] string genre, [FromQuery] bool onlyInUpcomingConcert)
        {
            var compositions = _compositionsRepo.Entities
                .Where(c => c.GroupId == GroupId
                            && (title == null || c.Title.Contains(title))
                            && (genre == null || c.Genre == genre));

            if (onlyInUpcomingConcert)
            {
                compositions = compositions.Intersect(
                    Repository.Get<Concert>().Entities
                        .Where(_ => _.GroupId == GroupId && _.Date > DateTime.UtcNow)
                        .SelectMany(_ => _.Compositions)
                );
            }

            if (UserGroupPosition.Director || UserGroupPosition.Manager)
                return Ok(await compositions.ProjectTo<CompositionData>(MapperConfig).ToListAsync());

            IEnumerable<Composition> result = await compositions.AsNoTracking()
                .Include(_ => _.Uploader)
                .Include(_ => _.SheetMusics)
                .ToListAsync();

            var rolesSet = new HashSet<int>(UserGroupPosition.Roles.Select(_ => _.Id));

            result = result.Where(c => c.SheetMusics.Any(s => rolesSet.Contains(s.RoleId)));

            return Ok(Mapper.Map<IEnumerable<CompositionData>>(result));
        }

        [HttpGet("{compositionId}")]
        [Authorize(Policy = GroupRolesPolicy.DirectorOrMember)]
        public async Task<IActionResult> GetFullComposition([FromRoute] int compositionId)
        {
            var composition = await SingleOrError(
                _compositionsRepo.FindOne(new CompositionIdentifier(GroupId, compositionId))
                    .Include(c => c.Uploader)
                    .Include(c => c.SheetMusics
                        .Where(s => UserGroupPosition.Director
                                 || UserGroupPosition.Roles.Any(r => r.Id == s.RoleId)))
                    .ThenInclude(_ => _.Role)
                );

            return Ok(Mapper.Map<FullCompositionData>(composition));
        }
    }
}
