using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class valuecomparer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "Courses",
                newName: "skills");

            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "Coaches",
                newName: "skills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "skills",
                table: "Courses",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "skills",
                table: "Coaches",
                newName: "Skills");
        }
    }
}
