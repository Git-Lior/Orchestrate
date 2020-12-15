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
            modelBuilder.Entity<AssignedRole>().HasKey(ar => new { ar.GroupID, ar.RoleID, ar.UserID });
            modelBuilder.Entity<Composition>().HasKey(c => new { c.GroupID, c.CompositionID });
            modelBuilder.Entity<Concert>().HasKey(c => new { c.GroupID, c.ConcertID });
            modelBuilder.Entity<ConcertAttendance>().HasKey(ca => new { ca.GroupID, ca.ConcertID, ca.UserID });
            modelBuilder.Entity<ConcertComposition>().HasKey(cc => new { cc.GroupID, cc.ConcertID, cc.CompositionID });
            modelBuilder.Entity<SheetMusic>().HasKey(sm => new { sm.GroupID, sm.CompositionID, sm.RoleID });
            modelBuilder.Entity<SheetMusicComment>().HasKey(smm => new { smm.GroupID, smm.CompositionID, smm.RoleID, smm.CommentID });

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
                .HasForeignKey(ar => ar.UserID);

            modelBuilder.Entity<AssignedRole>()
                .HasOne(ar => ar.Group)
                .WithMany(g => g.AssignedRoles)
                .HasForeignKey(ar => ar.GroupID);

            modelBuilder.Entity<AssignedRole>()
                .HasOne(ar => ar.Role)
                .WithMany()
                .HasForeignKey(ar => ar.RoleID);
        }
    }
}
