using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class collectionaddedinIdea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdeaImages",
                table: "Idea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdeaImages",
                table: "Idea",
                nullable: true);
        }
    }
}
