using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Orchestrate.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Section = table.Column<string>(type: "text", nullable: true),
                    RoleNum = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ManagerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_group_user_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assigned_role",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_role", x => new { x.GroupId, x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_assigned_role_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_assigned_role_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_assigned_role_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "composition",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    CompositionId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Composer = table.Column<string>(type: "text", nullable: true),
                    Genre = table.Column<string>(type: "text", nullable: true),
                    UploaderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_composition", x => new { x.GroupId, x.CompositionId });
                    table.ForeignKey(
                        name: "FK_composition_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_composition_user_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "concert",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ConcertId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concert", x => new { x.GroupId, x.ConcertId });
                    table.ForeignKey(
                        name: "FK_concert_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_director",
                columns: table => new
                {
                    DirectorOfGroupsGroupId = table.Column<int>(type: "integer", nullable: false),
                    DirectorsUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_director", x => new { x.DirectorOfGroupsGroupId, x.DirectorsUserId });
                    table.ForeignKey(
                        name: "FK_group_director_group_DirectorOfGroupsGroupId",
                        column: x => x.DirectorOfGroupsGroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_director_user_DirectorsUserId",
                        column: x => x.DirectorsUserId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_role",
                columns: table => new
                {
                    AvailableRolesRoleId = table.Column<int>(type: "integer", nullable: false),
                    InGroupsGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_role", x => new { x.AvailableRolesRoleId, x.InGroupsGroupId });
                    table.ForeignKey(
                        name: "FK_group_role_group_InGroupsGroupId",
                        column: x => x.InGroupsGroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_role_role_AvailableRolesRoleId",
                        column: x => x.AvailableRolesRoleId,
                        principalTable: "role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sheet_music",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    CompositionId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    File = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheet_music", x => new { x.GroupId, x.CompositionId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_sheet_music_composition_GroupId_CompositionId",
                        columns: x => new { x.GroupId, x.CompositionId },
                        principalTable: "composition",
                        principalColumns: new[] { "GroupId", "CompositionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "concert_attendance",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ConcertId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Attending = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concert_attendance", x => new { x.GroupId, x.ConcertId, x.UserId });
                    table.ForeignKey(
                        name: "FK_concert_attendance_concert_GroupId_ConcertId",
                        columns: x => new { x.GroupId, x.ConcertId },
                        principalTable: "concert",
                        principalColumns: new[] { "GroupId", "ConcertId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_concert_attendance_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_concert_attendance_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "concert_composition",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ConcertId = table.Column<int>(type: "integer", nullable: false),
                    CompositionId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concert_composition", x => new { x.GroupId, x.ConcertId, x.CompositionId });
                    table.ForeignKey(
                        name: "FK_concert_composition_composition_GroupId_CompositionId",
                        columns: x => new { x.GroupId, x.CompositionId },
                        principalTable: "composition",
                        principalColumns: new[] { "GroupId", "CompositionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_concert_composition_concert_GroupId_ConcertId",
                        columns: x => new { x.GroupId, x.ConcertId },
                        principalTable: "concert",
                        principalColumns: new[] { "GroupId", "ConcertId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_concert_composition_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sheet_music_comment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    CompositionId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheet_music_comment", x => new { x.GroupId, x.CompositionId, x.RoleId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_sheet_music_comment_composition_GroupId_CompositionId",
                        columns: x => new { x.GroupId, x.CompositionId },
                        principalTable: "composition",
                        principalColumns: new[] { "GroupId", "CompositionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_comment_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_comment_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_comment_sheet_music_GroupId_CompositionId_RoleId",
                        columns: x => new { x.GroupId, x.CompositionId, x.RoleId },
                        principalTable: "sheet_music",
                        principalColumns: new[] { "GroupId", "CompositionId", "RoleId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_music_comment_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assigned_role_RoleId",
                table: "assigned_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_assigned_role_UserId",
                table: "assigned_role",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_composition_UploaderId",
                table: "composition",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_concert_attendance_UserId",
                table: "concert_attendance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_concert_composition_GroupId_CompositionId",
                table: "concert_composition",
                columns: new[] { "GroupId", "CompositionId" });

            migrationBuilder.CreateIndex(
                name: "IX_group_ManagerId",
                table: "group",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_group_director_DirectorsUserId",
                table: "group_director",
                column: "DirectorsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_group_role_InGroupsGroupId",
                table: "group_role",
                column: "InGroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_music_RoleId",
                table: "sheet_music",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_music_comment_RoleId",
                table: "sheet_music_comment",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_music_comment_UserId",
                table: "sheet_music_comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_Email",
                table: "user",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assigned_role");

            migrationBuilder.DropTable(
                name: "concert_attendance");

            migrationBuilder.DropTable(
                name: "concert_composition");

            migrationBuilder.DropTable(
                name: "group_director");

            migrationBuilder.DropTable(
                name: "group_role");

            migrationBuilder.DropTable(
                name: "sheet_music_comment");

            migrationBuilder.DropTable(
                name: "concert");

            migrationBuilder.DropTable(
                name: "sheet_music");

            migrationBuilder.DropTable(
                name: "composition");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
