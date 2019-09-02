using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class Mig9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Idea");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Idea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Idea");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Idea",
                nullable: false,
                defaultValue: 0);
        }
    }
}
