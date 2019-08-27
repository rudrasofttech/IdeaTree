using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class Mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Newsletter",
                table: "Member",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Newsletter",
                table: "Member");
        }
    }
}
