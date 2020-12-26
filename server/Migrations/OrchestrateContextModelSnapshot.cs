﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Orchestrate.API.Data;

namespace Orchestrate.API.Migrations
{
    [DbContext(typeof(OrchestrateContext))]
    partial class OrchestrateContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("GroupRole", b =>
                {
                    b.Property<int>("AvailableRolesRoleId")
                        .HasColumnType("integer");

                    b.Property<int>("InGroupsGroupId")
                        .HasColumnType("integer");

                    b.HasKey("AvailableRolesRoleId", "InGroupsGroupId");

                    b.HasIndex("InGroupsGroupId");

                    b.ToTable("group_role");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("DirectorOfGroupsGroupId")
                        .HasColumnType("integer");

                    b.Property<int>("DirectorsUserId")
                        .HasColumnType("integer");

                    b.HasKey("DirectorOfGroupsGroupId", "DirectorsUserId");

                    b.HasIndex("DirectorsUserId");

                    b.ToTable("group_director");
                });

            modelBuilder.Entity("Orchestrate.API.Models.AssignedRole", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "RoleId", "UserId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("assigned_role");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Composition", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("CompositionId")
                        .HasColumnType("integer");

                    b.Property<string>("Composer")
                        .HasColumnType("text");

                    b.Property<string>("Genre")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("UploaderId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "CompositionId");

                    b.HasIndex("UploaderId");

                    b.ToTable("composition");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Concert", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("ConcertId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("GroupId", "ConcertId");

                    b.ToTable("concert");
                });

            modelBuilder.Entity("Orchestrate.API.Models.ConcertAttendance", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("ConcertId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<bool>("Attending")
                        .HasColumnType("boolean");

                    b.HasKey("GroupId", "ConcertId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("concert_attendance");
                });

            modelBuilder.Entity("Orchestrate.API.Models.ConcertComposition", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("ConcertId")
                        .HasColumnType("integer");

                    b.Property<int>("CompositionId")
                        .HasColumnType("integer");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "ConcertId", "CompositionId");

                    b.HasIndex("GroupId", "CompositionId");

                    b.ToTable("concert_composition");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("GroupId");

                    b.HasIndex("ManagerId");

                    b.ToTable("group");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("RoleNum")
                        .HasColumnType("integer");

                    b.Property<string>("Section")
                        .HasColumnType("text");

                    b.HasKey("RoleId");

                    b.ToTable("role");
                });

            modelBuilder.Entity("Orchestrate.API.Models.SheetMusic", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("CompositionId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("File")
                        .HasColumnType("bytea");

                    b.HasKey("GroupId", "CompositionId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("sheet_music");
                });

            modelBuilder.Entity("Orchestrate.API.Models.SheetMusicComment", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("CompositionId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("CommentId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "CompositionId", "RoleId", "CommentId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("sheet_music_comment");
                });

            modelBuilder.Entity("Orchestrate.API.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("GroupRole", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("AvailableRolesRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("InGroupsGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("DirectorOfGroupsGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.User", null)
                        .WithMany()
                        .HasForeignKey("DirectorsUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Orchestrate.API.Models.AssignedRole", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany("AssignedRoles")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Composition", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany("Compositions")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.User", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Concert", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany("Concerts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Orchestrate.API.Models.ConcertAttendance", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Concert", "Concert")
                        .WithMany("Attendances")
                        .HasForeignKey("GroupId", "ConcertId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Concert");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Orchestrate.API.Models.ConcertComposition", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Composition", "Composition")
                        .WithMany()
                        .HasForeignKey("GroupId", "CompositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Concert", "Concert")
                        .WithMany("Compositions")
                        .HasForeignKey("GroupId", "ConcertId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Composition");

                    b.Navigation("Concert");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Group", b =>
                {
                    b.HasOne("Orchestrate.API.Models.User", "Manager")
                        .WithMany("ManagingGroups")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("Orchestrate.API.Models.SheetMusic", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Composition", "Composition")
                        .WithMany()
                        .HasForeignKey("GroupId", "CompositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Composition");

                    b.Navigation("Group");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Orchestrate.API.Models.SheetMusicComment", b =>
                {
                    b.HasOne("Orchestrate.API.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.Composition", "Composition")
                        .WithMany()
                        .HasForeignKey("GroupId", "CompositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Orchestrate.API.Models.SheetMusic", null)
                        .WithMany("Comments")
                        .HasForeignKey("GroupId", "CompositionId", "RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Composition");

                    b.Navigation("Group");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Concert", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("Compositions");
                });

            modelBuilder.Entity("Orchestrate.API.Models.Group", b =>
                {
                    b.Navigation("AssignedRoles");

                    b.Navigation("Compositions");

                    b.Navigation("Concerts");
                });

            modelBuilder.Entity("Orchestrate.API.Models.SheetMusic", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Orchestrate.API.Models.User", b =>
                {
                    b.Navigation("ManagingGroups");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
