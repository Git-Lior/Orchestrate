using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Models;

namespace Orchestrate.API.Data
{
    public class OrchestrateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
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
            modelBuilder.Entity<Role>().HasIndex(r => new { r.Section, r.Num }).IsUnique();

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Directors)
                .WithMany(u => u.DirectorOfGroups)
                .UsingEntity(j => j.ToTable("group_director"));

            modelBuilder.Entity<GroupRole>()
                .HasMany(_ => _.Members)
                .WithMany(_ => _.MemberOfGroups)
                .UsingEntity(j => j.ToTable("group_role_member"));

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
                .WithOwner();

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Attendances)
                .WithOne(_ => _.Concert)
                .HasForeignKey(c => new { c.GroupId, c.ConcertId });

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Compositions)
                .WithMany(c => c.Concerts)
                .UsingEntity(j => j.ToTable("concert_composition"));
        }
    }
}
