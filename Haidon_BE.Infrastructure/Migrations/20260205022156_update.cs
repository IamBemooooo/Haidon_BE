using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haidon_BE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Criteria_Users_UserId",
                table: "Criteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Criteria",
                table: "Criteria");

            migrationBuilder.RenameTable(
                name: "Criteria",
                newName: "Criterias");

            migrationBuilder.RenameIndex(
                name: "IX_Criteria_UserId",
                table: "Criterias",
                newName: "IX_Criterias_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Criterias",
                table: "Criterias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Criterias_Users_UserId",
                table: "Criterias",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Criterias_Users_UserId",
                table: "Criterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Criterias",
                table: "Criterias");

            migrationBuilder.RenameTable(
                name: "Criterias",
                newName: "Criteria");

            migrationBuilder.RenameIndex(
                name: "IX_Criterias_UserId",
                table: "Criteria",
                newName: "IX_Criteria_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Criteria",
                table: "Criteria",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Criteria_Users_UserId",
                table: "Criteria",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
