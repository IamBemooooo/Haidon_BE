using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haidon_BE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnonymousAvatar",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "RevealedAvatar",
                table: "UserProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnonymousAvatar",
                table: "UserProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevealedAvatar",
                table: "UserProfiles",
                type: "text",
                nullable: true);
        }
    }
}
