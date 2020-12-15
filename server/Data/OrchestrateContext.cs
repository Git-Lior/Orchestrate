using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Data
{
    public class OrchestrateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }

        public OrchestrateContext(DbContextOptions<OrchestrateContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // composite keys
            modelBuilder.Entity<AssignedRole>().HasKey(ar => new { ar.GroupId, ar.RoleId, ar.UserId });
            modelBuilder.Entity<Composition>().HasKey(c => new { c.GroupId, c.CompositionId });
            modelBuilder.Entity<Concert>().HasKey(c => new { c.GroupId, c.ConcertId });
            modelBuilder.Entity<ConcertAttendance>().HasKey(ca => new { ca.GroupId, ca.ConcertId, ca.UserId });
            modelBuilder.Entity<ConcertComposition>().HasKey(cc => new { cc.GroupId, cc.ConcertId, cc.CompositionId });
            modelBuilder.Entity<SheetMusic>().HasKey(sm => new { sm.GroupId, sm.CompositionId, sm.RoleId });
            modelBuilder.Entity<SheetMusicComment>().HasKey(smc => new { smc.GroupId, smc.CompositionId, smc.RoleId, smc.CommentId });

            modelBuilder.Entity<User>()
                .HasMany(u => u.DirectorOfGroups)
                .WithMany(g => g.Directors)
                .UsingEntity(j => j.ToTable("group_director"));

            modelBuilder.Entity<Group>()
                .HasMany(g => g.AvailableRoles)
                .WithMany(r => r.InGroups)
                .UsingEntity(j => j.ToTable("group_role"));

            modelBuilder.Entity<AssignedRole>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ar => ar.UserId);

            modelBuilder.Entity<AssignedRole>()
                .HasOne(ar => ar.Group)
                .WithMany(g => g.AssignedRoles)
                .HasForeignKey(ar => ar.GroupId);

            modelBuilder.Entity<AssignedRole>()
                .HasOne(ar => ar.Role)
                .WithMany()
                .HasForeignKey(ar => ar.RoleId);

            modelBuilder.Entity<SheetMusic>()
                .HasMany(sm => sm.Comments)
                .WithOne()
                .HasForeignKey(smc => new { smc.GroupId, smc.CompositionId, smc.RoleId });

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Attendances)
                .WithOne(ca => ca.Concert)
                .HasForeignKey(ca => new { ca.GroupId, ca.ConcertId });

            modelBuilder.Entity<Concert>()
                .HasMany(c => c.Compositions)
                .WithOne(c => c.Concert)
                .HasForeignKey(cc => new { cc.GroupId, cc.ConcertId });
        }
    }
}
