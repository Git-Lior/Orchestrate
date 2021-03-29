using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Repositories.Interfaces;
using Orchestrate.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orchestrate.Data.Models.Joins;

namespace Orchestrate.Data.Repositories
{
    public class ConcertsRepository : EntityRepositoryBase<Concert>, IConcertsRepository
    {
        protected override DbSet<Concert> DbEntities => Context.Concerts;

        public ConcertsRepository(OrchestrateContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<Concert> FindOne(object identifier)
        {
            if (identifier is not ConcertIdentifier c) throw new InvalidOperationException("invalid concert identifier");

            return DbEntities.Where(_ => _.GroupId == c.GroupId && _.Id == c.ConcertId);
        }

        public async Task AddComposition(Concert concert, Composition composition)
        {
            if (concert.Compositions.Any(_ => _.Id == composition.Id))
                throw new ArgumentException("Composition already in concert");

            concert.Compositions.Add(composition);

            await Context.SaveChangesAsync();
        }

        public async Task RemoveComposition(Concert concert, Composition composition)
        {
            if (concert.Compositions.All(_ => _.Id != composition.Id))
                throw new ArgumentException("Composition not in concert");

            concert.Compositions.Remove(composition);

            await Context.SaveChangesAsync();
        }

        public async Task SetUserAttendance(Concert concert, int userId, bool attending)
        {
            var attendance = concert.Attendances.FirstOrDefault(_ => _.UserId == userId);

            if (attendance == null)
                concert.Attendances.Add(new ConcertAttendance { UserId = userId, UpdatedAt = DateTime.UtcNow, Attending = attending });
            else
            {
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.Attending = attending;
            }

            await Context.SaveChangesAsync();
        }
    }
}
