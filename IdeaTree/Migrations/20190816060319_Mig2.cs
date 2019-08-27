using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class Mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Member");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogon",
                table: "Member",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogon",
                table: "Member");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Member",
                maxLength: 250,
                nullable: true);
        }
    }
}
