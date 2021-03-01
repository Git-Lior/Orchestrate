using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Models;

namespace Orchestrate.API.Data
{
    public class OrchestrateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<SheetMusic> SheetMusics { get; set; }
        public DbSet<Concert> Concerts { get; set; }

        public OrchestrateContext(DbContextOptions<OrchestrateContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // composite keys
            modelBuilder.Entity<GroupMember>().HasKey(ar => new { ar.GroupId, ar.RoleId, ar.UserId });
            modelBuilder.Entity<Composition>().HasKey(c => new { c.GroupId, c.Id });
            modelBuilder.Entity<Concert>().HasKey(c => new { c.GroupId, c.Id });
            modelBuilder.Entity<SheetMusic>().HasKey(sm => new { sm.GroupId, sm.CompositionId, sm.RoleId });
            modelBuilder.Entity<ConcertComposition>().HasKey(c => new { c.GroupId, c.CompositionId, c.ConcertId });

            modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);
            modelBuilder.Entity<Role>().HasAlternateKey(r => new { r.Section, r.Num });

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Directors)
                .WithMany(u => u.DirectorOfGroups)
                .UsingEntity(j => j.ToTable("group_director"));

            modelBuilder.Entity<Composition>()
                .HasMany(c => c.SheetMusics)
                .WithOne(sm => sm.Composition)
                .HasForeignKey(sm => new { sm.GroupId, sm.CompositionId });

            modelBuilder.Entity<Composition>()
                .HasMany(c => c.ConcertCompositions)
                .WithOne(cc => cc.Composition)
                .HasForeignKey(cc => new { cc.GroupId, cc.CompositionId });

            modelBuilder.Entity<SheetMusic>()
                .OwnsMany(sm => sm.Comments)
                .WithOwner();

            modelBuilder.Entity<Concert>()
                .OwnsMany(c => c.Attendances)
                .WithOwner();

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.ConcertCompositions)
                .WithOne(cc => cc.Concert)
                .HasForeignKey(cc => new { cc.GroupId, cc.ConcertId });
        }
    }
}
