using Microsoft.EntityFrameworkCore.Migrations;

namespace Orchestrate.API.Migrations
{
    public partial class UserTempPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordTemporary",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPasswordTemporary",
                table: "user");
        }
    }
}
