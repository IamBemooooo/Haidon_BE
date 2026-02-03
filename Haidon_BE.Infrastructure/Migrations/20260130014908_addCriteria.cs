using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haidon_BE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgeFrom = table.Column<int>(type: "integer", nullable: true),
                    AgeTo = table.Column<int>(type: "integer", nullable: true),
                    IsMale = table.Column<bool>(type: "boolean", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Criteria_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_UserId",
                table: "Criteria",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Criteria");
        }
    }
}
