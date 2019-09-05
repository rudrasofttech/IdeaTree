using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class Mig12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Member",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Member",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Member");

            
        }
    }
}
