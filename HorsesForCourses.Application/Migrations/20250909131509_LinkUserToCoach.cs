using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.Application.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserToCoach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Coaches",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coaches");
        }
    }
}
