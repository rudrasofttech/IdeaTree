using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaTree.Migrations
{
    public partial class Mig6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModifiedByID",
                table: "Member",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Member",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_ModifiedByID",
                table: "Member",
                column: "ModifiedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Member_ModifiedByID",
                table: "Member",
                column: "ModifiedByID",
                principalTable: "Member",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Member_ModifiedByID",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_ModifiedByID",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "ModifiedByID",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Member");
        }
    }
}
