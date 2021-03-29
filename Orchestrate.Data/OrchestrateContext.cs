using Microsoft.EntityFrameworkCore;
using Orchestrate.Data.Models;
using Orchestrate.Data.Models.Joins;

namespace Orchestrate.Data
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
            modelBuilder.Entity<GroupRole>().HasKey(ar => new { ar.GroupId, ar.RoleId });
            modelBuilder.Entity<Composition>().HasKey(c => new { c.GroupId, c.Id });
            modelBuilder.Entity<SheetMusic>().HasKey(sm => new { sm.GroupId, sm.CompositionId, sm.RoleId });
            modelBuilder.Entity<Concert>().HasKey(c => new { c.GroupId, c.Id });
            modelBuilder.Entity<ConcertAttendance>().HasKey(c => new { c.GroupId, c.ConcertId, c.UserId });

            modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);
            modelBuilder.Entity<Role>().HasAlternateKey(r => new { r.Section, r.Num });

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Directors)
                .WithMany(u => u.DirectorOfGroups)
                .UsingEntity<GroupDirector>(
                    j => j.HasOne(gd => gd.User).WithMany().HasForeignKey(gd => new { gd.UserId }),
                    j => j.HasOne(gd => gd.Group).WithMany().HasForeignKey(gd => new { gd.GroupId }),
                    j => j.HasKey(gd => new { gd.GroupId, gd.UserId })
                );

            modelBuilder.Entity<GroupRole>()
                .HasMany(_ => _.Members)
                .WithMany(_ => _.MemberOfGroups)
                .UsingEntity<GroupRoleMember>(
                j => j.HasOne(grm => grm.User).WithMany().HasForeignKey(grm => new { grm.UserId }),
                j => j.HasOne(grm => grm.GroupRole).WithMany().HasForeignKey(grm => new { grm.GroupId, grm.RoleId }),
                j => j.HasKey(grm => new { grm.GroupId, grm.RoleId, grm.UserId })
                );

            modelBuilder.Entity<GroupRole>()
                .HasMany(_ => _.SheetMusics)
                .WithOne(_ => _.GroupRole)
                .HasForeignKey(r => new { r.GroupId, r.RoleId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Composition>()
                .HasMany(c => c.SheetMusics)
                .WithOne(sm => sm.Composition)
                .HasForeignKey(sm => new { sm.GroupId, sm.CompositionId });

            modelBuilder.Entity<SheetMusic>()
                .OwnsMany(sm => sm.Comments)
                .WithOwner()
                .HasForeignKey(smc => new { smc.GroupId, smc.CompositionId, smc.RoleId });

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Attendances)
                .WithOne(_ => _.Concert)
                .HasForeignKey(c => new { c.GroupId, c.ConcertId });

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Compositions)
                .WithMany(c => c.Concerts)
                .UsingEntity<ConcertComposition>(
                    j => j.HasOne(_ => _.Composition).WithMany().HasForeignKey(c => new { c.GroupId, c.CompositionId }),
                    j => j.HasOne(_ => _.Concert).WithMany().HasForeignKey(c => new { c.GroupId, c.ConcertId }),
                    j => j.HasKey(cc => new { cc.GroupId, cc.ConcertId, cc.CompositionId })
                );
        }
    }
}
